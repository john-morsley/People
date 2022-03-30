//using Microsoft.Extensions.Hosting;

//using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Morsley.UK.People.Application.Events;
using Morsley.UK.People.Application.Handlers;
using Morsley.UK.People.Application.Interfaces;
using Morsley.UK.People.Messaging;
using Morsley.UK.People.Persistence.IoC;
using Morsley.UK.People.Persistence.Repositories;

namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class BusTestFixture
{
    protected DockerTestFixture<RabbitMQInDocker>? DockerTestFixture;
    //protected global::AutoFixture.Fixture? AutoFixture;

    public IConfiguration? Configuration;

    private IEventBus? _eventBus;

    private string _name;

    public BusTestFixture(string name)
    {
        //AutoFixture = new global::AutoFixture.Fixture();
        //AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        //AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        //_eventBus = new EventBus();

        _name = name;
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        LoadInitialConfiguration();

        var section = Configuration!.GetSection(nameof(RabbitMQSettings));
        var settings = section.Get<RabbitMQSettings>();

        if (settings == null)
        {
            Console.WriteLine("No configuration for RabbitMQ so exiting!");
            return;
        }

        if (!int.TryParse(settings.Port, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<RabbitMQInDocker>(_name, settings.Username!, settings.Password!, port);

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }

        LoadAdditionalConfiguration();

        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IEventBus, EventBus>();
            services.AddScoped<PersonAddedEventHandler>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddPersistence(Configuration);
        });

        var host = builder.Build();

        var factory = host.Services.GetService<IServiceScopeFactory>();
        
        _eventBus = new EventBus(Configuration, factory!, null);
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

    protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

        IConfiguration configuration = builder.Build();

        return configuration;
    }

    public Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        if (DockerTestFixture != null) additional.Add("RabbitMQSettings:Port", ContainerPort.ToString());
        return additional;
    }

    private void LoadAdditionalConfiguration()
    {
        var additionalConfiguration = GetInMemoryConfiguration();
        Configuration = GetConfiguration(additionalConfiguration);
    }

    protected void LoadInitialConfiguration()
    {
        Configuration = GetConfiguration();
    }
}
