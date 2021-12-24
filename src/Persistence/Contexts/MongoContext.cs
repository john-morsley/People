﻿namespace Persistence.Contexts;

public class MongoContext : IMongoContext
{
    private readonly IConfiguration _configuration;
    private readonly List<Func<Task>> _commands;
    private MongoSettings MongoSettings;

    private IMongoDatabase Database { get; set; }

    public MongoClient MongoClient { get; set; }

    public IClientSessionHandle Session { get; set; }

    public MongoContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _commands = new List<Func<Task>>();
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();

        return Database.GetCollection<T>(name);
    }

    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ConfigureMongo()
    {
        if (MongoClient != null) return;
        
        var section = _configuration.GetSection(nameof(MongoSettings));
        MongoSettings = section.Get<MongoSettings>();

        //var connectionString = GetConnectionString(MongoSettings);
        MongoClient = new MongoClient(ConnectionString);
        Database = MongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]);
    }

    public bool IsHealthy()
    {
        try
        {
            ConfigureMongo();
            
            // Other stuff we might need to do in the future!?

            return true;
        }
        catch (Exception e)
        {
            // ToDo --> Log e            
        }
        return false;
    }

    public string ConnectionString => GetConnectionString(MongoSettings);

    private string GetConnectionString(MongoSettings mongoSettings)
    {
        return $"mongodb://{mongoSettings.Username}:{mongoSettings.Password}@{mongoSettings.Host}:{mongoSettings.Port}";
    }
}