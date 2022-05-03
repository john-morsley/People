namespace Morsley.UK.People.Persistence.Contexts;

public class MongoContext : IMongoContext
{
    private readonly IConfiguration _configuration;
    private readonly string _persistenceKey;
    //private readonly List<Func<Task>> _commands;
    private MongoDBSettings? _mongoSettings;

    private IMongoDatabase? Database { get; set; }

    public MongoClient? MongoClient { get; set; }

    public IClientSessionHandle? Session { get; set; }

    public MongoContext(IConfiguration configuration, string persistenceKey)
    {
        _configuration = configuration;
        _persistenceKey = persistenceKey;
        //_commands = new List<Func<Task>>();
    }

    //public void AddCommand(Func<Task> func)
    //{
    //    _commands.Add(func);
    //}

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();

        if (Database is null) throw new InvalidOperationException("Database cannot be null.");

        return Database.GetCollection<T>(name);
    }

    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ConfigureMongo()
    {
        if (MongoClient is not null) return;
        
        var section = _configuration.GetSection(_persistenceKey);
        _mongoSettings = section.Get<MongoDBSettings>();

        var connectionString = GetConnectionString();
        MongoClient = new MongoClient(connectionString);
        Database = MongoClient.GetDatabase(_mongoSettings.DatabaseName);
    }

    public bool IsHealthy()
    {
        try
        {
            ConfigureMongo();
            
            // Other stuff we might need to do in the future!?

            return true;
        }
        catch //(Exception e)
        {
            // ToDo --> Log e            
        }
        return false;
    }

    public string GetConnectionString()
    {
        if (_mongoSettings is null) throw new InvalidOperationException("MongoSettings cannot be null.");
        return GetConnectionString(_mongoSettings);
    }

    private string GetConnectionString(MongoDBSettings mongoDbSettings)
    {
        return $"mongodb://{mongoDbSettings.Username}:{mongoDbSettings.Password}@{mongoDbSettings.Host}:{mongoDbSettings.Port}";
    }
}