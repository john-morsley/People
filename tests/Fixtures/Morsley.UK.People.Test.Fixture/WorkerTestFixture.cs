namespace Morsley.UK.People.Test.Fixture;

public class WorkerTestFixture<TProgram> where TProgram : class
{
    private readonly IConfiguration _configuration;
    private readonly string _persistenceKey;

    private IHost? _host;

    public WorkerTestFixture(IConfiguration configuration, string persistenceKey)
    {
        _configuration = configuration;
        _persistenceKey = persistenceKey;
    }

    [OneTimeSetUp]
    protected async virtual Task OneTimeSetUp()
    {
        //LoadInitialConfiguration();

        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IEventBus, EventBus>();
            services.AddScoped<PersonAddedEventHandler>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddPersistence(_configuration, _persistenceKey);
        });

        _host = builder.Build();

        //var factory = host.Services.GetService<IServiceScopeFactory>();
    }

    [SetUp]
    protected virtual void SetUp()
    {
    }

    [TearDown]
    protected virtual void TearDown()
    { 
        _host?.Dispose();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
    }
}
