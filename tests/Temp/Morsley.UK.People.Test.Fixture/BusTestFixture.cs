//using Microsoft.Extensions.Hosting;

//using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Morsley.UK.People.Application.Events;
using Morsley.UK.People.Application.Handlers;
using Morsley.UK.People.Application.Interfaces;
using Morsley.UK.People.Application.IoC;
using Morsley.UK.People.Messaging;
using Morsley.UK.People.Persistence.IoC;
using Morsley.UK.People.Persistence.Repositories;

namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class BusTestFixture
{
    protected DockerTestFixture<RabbitMQInDocker>? DockerTestFixture;
    //protected global::AutoFixture.Fixture? AutoFixture;

    private IConfiguration? _configuration;

    public IConfiguration? Configuration => _configuration;

    private readonly string _persistenceKey;

    private IEventBus? _eventBus;

    private string _name;

    public BusTestFixture(string name, IConfiguration configuration, string persistenceKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");
        if (configuration == null) throw new ArgumentNullException("configuration");

        //AutoFixture = new global::AutoFixture.Fixture();
        //AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        //AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        //_eventBus = new EventBus();

        _name = name;
        _configuration = configuration;
        _persistenceKey = persistenceKey;
    }

    public async Task CreateBus()
    {
        //LoadInitialConfiguration();

        //var section = _configuration!.GetSection(nameof(RabbitMQSettings));
        //var settings = section.Get<RabbitMQSettings>();

        //var settings = _configuration["RabbitMQSettings"];

        var potentialPort = _configuration["RabbitMQSettings:Port"];
        var username = _configuration["RabbitMQSettings:Username"];
        var password = _configuration["RabbitMQSettings:Password"];

        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<RabbitMQInDocker>(_name, username!, password!, port);

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }

        UpdateConfiguration();

        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            //services.AddSingleton<IEventBus, EventBus>();
            services.AddScoped<PersonAddedEventHandler>();
            //services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddApplication();
            services.AddPersistence(_configuration, _persistenceKey);
        });

        var host = builder.Build();

        var factory = host.Services.GetService<IServiceScopeFactory>();
        
        _eventBus = new EventBus(_configuration, factory!, null);
    }

    private void UpdateConfiguration()
    {
        var builder = new ConfigurationBuilder().AddConfiguration(_configuration);
        builder.AddInMemoryCollection(GetInMemoryConfiguration());
        var configuration = builder.Build();
        var potentialPort = configuration["RabbitMQSettings:Port"];
        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }
        if (port != DockerTestFixture!.InDocker.Port)
        {
            throw new NotImplementedException("Port has not been updated!");
        }
        _configuration = configuration;
    }

    [SetUp]
    public virtual void SetUp()
    {
        //DeleteAllPeopleFromDatabase();
    }

    [TearDown]
    public virtual void TearDown()
    {
        //DeleteAllPeopleFromDatabase();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DockerTestFixture?.RunAfterTests();
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        //var eventName = typeof(T).Name;
        //var handlerType = typeof(TH);
        _eventBus!.Subscribe<T, TH>();
    }

    protected int ContainerPort
    {
        get
        {
            if (DockerTestFixture == null) throw new InvalidOperationException("Docker has not yet been initialised!");
            return DockerTestFixture!.GetContainerPort();
        }
    }

    //protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    //{
    //    var builder = new ConfigurationBuilder();

    //    builder.AddJsonFile("appsettings.json");

    //    if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

    //    IConfiguration configuration = builder.Build();

    //    return configuration;
    //}

    public Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        if (DockerTestFixture != null) additional.Add("RabbitMQSettings:Port", ContainerPort.ToString());
        return additional;
    }

    //private void LoadAdditionalConfiguration()
    //{
        //var additionalConfiguration = GetInMemoryConfiguration();
        //_configuration = GetConfiguration(additionalConfiguration);
        //var builder = new ConfigurationBuilder();
        //builder.
        //builder.AddInMemoryCollection(additionalConfiguration);
        //_configuration = builder.Build();
        //var manager = new ConfigurationManager();
        //manager.
    //}

    //protected void LoadInitialConfiguration()
    //{
    //    Configuration = GetConfiguration();
    //}
}
