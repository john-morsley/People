

//namespace API.Shared.Tests;

////[SetUpFixture]
//public class TestFixture
//{
//    //private static IConfigurationRoot _configuration; 
    
//    private string? _dockerContainerId;
//    private int _dockerContainerPort;
//    private readonly string _username;
//    private readonly string _password;

//    //public MongoSettings _mongoSsettings { get; }

//    public TestFixture(string username, string password)
//    {
//        _username = username;
//        _password = password;
//        //_mongoSsettings = settings;

//    }

//    //[OneTimeSetUp]
//    public async Task RunBeforeTests()
//    {
//        (_dockerContainerId, var dockerContainerPort) = 
//            await MongoDBInDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync(_username, _password);

//        _dockerContainerPort = Convert.ToInt32(dockerContainerPort);

//        //var context = new MongoContext();

//        //EnsureDatabase();
//    }

//    //[OneTimeTearDown]
//    public async Task RunAfterTests()
//    {
//        await MongoDBInDocker.EnsureDockerContainersStoppedAndRemovedAsync(_dockerContainerId);
//    }

//    public int GetContainerPort()
//    {
//        //var port = await MongoDBInDocker.GetPort();
//        //await _checkpoint.Reset(_configuration.GetConnectionString("AccessioningDbContext"));
//        //throw new NotImplementedException();
//        //return string.Empty;
//        //throw new NotImplementedException();

//        return _dockerContainerPort;
//    }

//    //private static void EnsureDatabase()
//    //{
//    //    using var scope = _scopeFactory.CreateScope();

//    //    var context = scope.ServiceProvider.GetService<AccessioningDbContext>();

//    //    context.Database.Migrate();
//    //}

//    public string MongoDBConnectionString
//    {
//        get { return MongoDBInDocker.ConnectionString(_username, _password, _dockerContainerPort); }
//    }

//    //public string DatabaseName 
//    //{
//    //    get { return MongoDBInDocker. }
//    //}

    
//}
