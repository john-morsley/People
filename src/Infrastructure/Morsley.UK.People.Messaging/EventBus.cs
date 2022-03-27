namespace Morsley.UK.People.Messaging;

public class EventBus : IEventBus
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;

    private RabbitMQSettings? _rabbitMQSettings;

    private readonly Dictionary<string, List<Type>> _eventHandlers;
    private readonly List<Type> _eventTypes;

    public EventBus(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ILogger logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        var section = configuration.GetSection(nameof(RabbitMQSettings));
        _rabbitMQSettings = section.Get<RabbitMQSettings>();

        // ToDo --> Are the above settings valid?

        _eventTypes = new List<Type>();
        _eventHandlers = new Dictionary<string, List<Type>>();
    }

    /// <summary>
    /// Publish an Event to the Event Bus
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Publish<T>(T @event) where T : Event
    {
        var factory = GetConnectionFactory<T>();

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueBind(_rabbitMQSettings!.QueueName, _rabbitMQSettings!.QueueName, null);
        var eventName = @event.GetType().Name;
        channel.QueueDeclare(_rabbitMQSettings!.QueueName, false, false, false, null);
        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("", eventName, null, body);
    }

    /// <summary>
    /// Subscribe to an Event with an EventHandler
    /// </summary>
    /// <typeparam name="T">The Event to subscribe to.</typeparam>
    /// <typeparam name="TH">The EventHandler to be called when the event happens.</typeparam>
    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers.Add(eventName, new List<Type>());
        }

        if (_eventHandlers[eventName].Any(s => s == handlerType))
        {
            _logger.Error($"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
            throw new ArgumentException($"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
        }

        _eventHandlers[eventName].Add(handlerType);

        StartBasicConsumer<T>();
    }

    private void StartBasicConsumer<T>()
    {
        var factory = GetConnectionFactory<T>();

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        var eventName = typeof(T).Name;
        channel.QueueDeclare(_rabbitMQSettings!.QueueName, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += ConsumerReceived;

        channel.BasicConsume(_rabbitMQSettings!.QueueName, false, consumer);
    }

    private ConnectionFactory GetConnectionFactory<T>()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMQSettings!.Host,
            UserName = _rabbitMQSettings!.Username,
            Password = _rabbitMQSettings!.Password,
            DispatchConsumersAsync = true
        };
        return factory;
    }

    private async Task ConsumerReceived(object sender, BasicDeliverEventArgs args)
    {
        var eventName = args.RoutingKey;
        var message = Encoding.UTF8.GetString(args.Body.Span);

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
            ((AsyncDefaultBasicConsumer)sender).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Something went wrong with Consumer_Received!");
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var subscriptions = _eventHandlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription);

                    if (handler == null) continue;

                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await ((Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event }));

                }
            }
        }
    }
}
