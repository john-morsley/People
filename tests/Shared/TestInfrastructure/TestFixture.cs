namespace Shared.TestInfrastructure;

public class TestFixture
{
    //private static IConfigurationRoot _configuration; 

    private string? _dockerContainerId;
    private int _dockerContainerPort;

    //public MongoSettings _mongoSsettings { get; }

    private readonly string _username;
    private readonly string _password;

    public TestFixture(string username, string password)
    {
        //_mongoSsettings = settings;
        _username = username;
        _password = password;
    }

    public async Task RunBeforeTests()
    {
        (_dockerContainerId, var dockerContainerPort) = 
            await MongoDBInDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync(_username, _password);

        _dockerContainerPort = Convert.ToInt32(dockerContainerPort);

        //var context = new MongoContext();

        //EnsureDatabase();
    }

    public async Task RunAfterTests()
    {
        await MongoDBInDocker.EnsureDockerContainersStoppedAndRemovedAsync(_dockerContainerId);
    }

    public int GetContainerPort()
    {
        //var port = await MongoDBInDocker.GetPort();
        //await _checkpoint.Reset(_configuration.GetConnectionString("AccessioningDbContext"));
        //throw new NotImplementedException();
        //return string.Empty;
        //throw new NotImplementedException();

        return _dockerContainerPort;
    }

    //private static void EnsureDatabase()
    //{
    //    using var scope = _scopeFactory.CreateScope();

    //    var context = scope.ServiceProvider.GetService<AccessioningDbContext>();

    //    context.Database.Migrate();
    //}

    public string MongoDBConnectionString
    {
        get { return MongoDBInDocker.ConnectionString(_username, _password, _dockerContainerPort); }
    }

    //public string DatabaseName 
    //{
    //    get { return MongoDBInDocker. }
    //}


}
