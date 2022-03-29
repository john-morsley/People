namespace Morsley.UK.People.Messaging;

public class EventBus : IEventBus
{
    private const string ExchangeName = "direct_people";
    private const string RoutingKey = "people_only";

    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;

    //private RabbitMQSettings? _rabbitMQSettings;

    private readonly Dictionary<string, List<Type>> _eventHandlers;
    private readonly List<Type> _eventTypes;

    public EventBus(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ILogger logger)
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        //var section = configuration.GetSection(nameof(RabbitMQSettings));
        //_rabbitMQSettings = section.Get<RabbitMQSettings>();

        // ToDo --> Are the above settings valid?

        _eventTypes = new List<Type>();
        _eventHandlers = new Dictionary<string, List<Type>>();
    }

    private RabbitMQSettings GetRabbitMQSettings()
    {
        var section = _configuration.GetSection(nameof(RabbitMQSettings));
        var rabbitMQSettings = section.Get<RabbitMQSettings>();
        return rabbitMQSettings;
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
        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, ExchangeName, RoutingKey);
        var eventName = @event.GetType().Name;
        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(ExchangeName, RoutingKey, null, body);
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

        AddEventType<T, TH>();
        AddEventHandler<T, TH>(eventName, handlerType);

        StartBasicConsumer<T>();
    }

    private void AddEventHandler<T, TH>(string eventName, Type handlerType) where T : Event where TH : IEventHandler<T>
    {
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
    }

    private void AddEventType<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }
    }

    private void StartBasicConsumer<T>()
    {
        var factory = GetConnectionFactory<T>();
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.ExchangeDeclare("direct_people", ExchangeType.Direct);
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, ExchangeName, RoutingKey);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += ConsumerReceived;
        channel.BasicConsume(queueName, autoAck: true, consumer);
    }

    private ConnectionFactory GetConnectionFactory<T>()
    {
        var settings = GetRabbitMQSettings();
        var factory = new ConnectionFactory
        {
            HostName = settings!.Host,
            Port = int.Parse(settings!.Port),
            UserName = settings!.Username,
            Password = settings!.Password,
            DispatchConsumersAsync = true
        };
        return factory;
    }

    private async Task ConsumerReceived(object sender, BasicDeliverEventArgs args)
    {
        var exchangeName = args.Exchange;
        //var routingKey = args.RoutingKey;
        var message = Encoding.UTF8.GetString(args.Body.Span);
        var eventName = EventName(message);

        try
        {
            
            await ProcessEvent(exchangeName, eventName, message).ConfigureAwait(false);
            ((AsyncDefaultBasicConsumer)sender).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Something went wrong with ConsumerReceived!");
        }
    }

    private string EventName(string json)
    {
        var data = (JObject)JsonConvert.DeserializeObject(json);
        var typeName = data["TypeName"].Value<string>();
        return typeName;
        //return null;
    }

    private async Task ProcessEvent(string exchangeName, string eventName, string message)
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
