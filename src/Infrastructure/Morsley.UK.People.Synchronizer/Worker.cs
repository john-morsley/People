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
        var readSettings = GetReadSettings();
        var writeSettings = GetWriteSettings();

        var oldRead = 0L;
        var oldWrite = 0L;
        var force = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            try
            {
                var newRead = NumberOfPeopleInDatabase(readSettings);
                var newWrite = NumberOfPeopleInDatabase(writeSettings);
                
                if (newRead != oldRead || newWrite != oldWrite || force)
                {
                    force = false;
                    _logger.Information("Read: {read} | Write: {write}", newRead, newWrite);
                }
                oldRead = newRead;
                oldWrite = newWrite;
            }
            catch
            {
                _logger.Warning("Encountered an unexpected error! Is the infrastructure OK?");
                force = true;
            }
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