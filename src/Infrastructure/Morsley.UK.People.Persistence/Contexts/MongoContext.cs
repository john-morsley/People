namespace Morsley.UK.People.Persistence.Contexts
{
    public class MongoContext : IMongoContext
    {
        private readonly IConfiguration _configuration;
        private readonly List<Func<Task>> _commands;
        private MongoSettings? _mongoSettings;

        private IMongoDatabase? Database { get; set; }

        public MongoClient? MongoClient { get; set; }

        public IClientSessionHandle? Session { get; set; }

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

            if (Database == null) throw new InvalidOperationException("Database cannot be null.");

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
            _mongoSettings = section.Get<MongoSettings>();

            var connectionString = GetConnectionString();
            MongoClient = new MongoClient(connectionString);
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
            catch //(Exception e)
            {
                // ToDo --> Log e            
            }
            return false;
        }

        public string GetConnectionString()
        {
            if (_mongoSettings == null) throw new InvalidOperationException("MongoSettings cannot be null.");
            return GetConnectionString(_mongoSettings);
        }

        private string GetConnectionString(MongoSettings mongoSettings)
        {
            return $"mongodb://{mongoSettings.Username}:{mongoSettings.Password}@{mongoSettings.Host}:{mongoSettings.Port}";
        }
    }
}