namespace Morsley.UK.People.Synchronizer;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public Worker(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //var previousRead = 0L;
        //var previousWrite = 0L;
        var readSettings = GetReadSettings();
        var writeSettings = GetWriteSettings();

        while (!stoppingToken.IsCancellationRequested)
        {
            var read = NumberOfPeopleInDatabase(readSettings);
            var write = NumberOfPeopleInDatabase(writeSettings);
            //if (read > previousRead || write > previousWrite)
            //{
                _logger.Debug("Read: {read} | Write: {write}", read, write);
                //previousRead = read;
                //previousWrite = write;
            //}
            await Task.Delay(1000, stoppingToken);
        }
    }

    private (string Username, string Password, string Port, string Database, string Table) GetSettings(string section)
    {
        var username = _configuration[$"{section}MongoDBSettings:Username"];
        var password = _configuration[$"{section}MongoDBSettings:Password"];
        var port = _configuration[$"{section}MongoDBSettings:Port"];
        var database = _configuration[$"{section}MongoDBSettings:DatabaseName"];
        var table = _configuration[$"{section}MongoDBSettings:TableName"];
        return (username, password, port, database, table);
    }

    private (string Username, string Password, string Port, string Database, string Table) GetWriteSettings()
    {
        return GetSettings("Write");
    }

    private (string Username, string Password, string Port, string Database, string Table) GetReadSettings()
    {
        return GetSettings("Read");
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

    public string GetConnectionString(string username, string password, string port)
    {
        return $"mongodb://{username}:{password}@localhost:{port}";
    }

    private long NumberOfPeopleInDatabase((string Username, string Password, string Port, string Database, string Table) settings)
    {
        var connectionString = GetConnectionString(settings.Username, settings.Password, settings.Port);
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.Database);
        var peopleTable = database.GetCollection<Person>(settings.Table);
        var numberOfPeople = peopleTable.Find(_ => true).CountDocuments();
        return numberOfPeople;
    }
}