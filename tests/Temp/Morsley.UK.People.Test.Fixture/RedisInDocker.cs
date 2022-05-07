namespace Morsley.UK.People.Test.Fixture;

public class RedisInDocker : InDocker
{
    public const string REDIS_IMAGE = "redis";
    public const string REDIS_IMAGE_TAG = "7";
    //public const string MONGODB_CONTAINER_NAME = "IntegrationTesting_MongoDB";
    //public const string MONGODB_AUTHENTICATION_MECHANISM = "SCRAM-SHA-1";
    //public const int MONGODB_PORT = 27017;

    private ConnectionMultiplexer? _redis;

    public ConnectionMultiplexer? Redis
    {
        get
        {
            return _redis;
        }
        private set
        {
            if (value is null) throw new ArgumentNullException(nameof(value), "The value for Redis cannot be set to null!");
            _redis = value;
        }
    }

    public RedisInDocker(
        string containerName, 
        int port) : base(port)
    {
        _containerName =containerName;
    }

    private string _containerName;

    public async override Task<(string containerId, int port)> EnsureDockerStartedAndGetContainerIdAndPortAsync()
    {
        await CleanupRunningContainers();

        var dockerClient = GetDockerClient();
        var freePort = GetFreePort();

        // This call ensures that the latest Docker image is pulled
        var imagesCreateParameters = new ImagesCreateParameters { FromImage = $"{REDIS_IMAGE}:{REDIS_IMAGE_TAG}" };
        await dockerClient.Images.CreateImageAsync(imagesCreateParameters, null, new Progress<JSONMessage>());

        var container = await dockerClient
            .Containers
            .CreateContainerAsync(new CreateContainerParameters
            {
                Name = _containerName,
                Image = $"{REDIS_IMAGE}:{REDIS_IMAGE_TAG}",
                //Env = new List<string>
                //{
                //    $"MONGO_INITDB_ROOT_USERNAME={Username}",
                //    $"MONGO_INITDB_ROOT_PASSWORD={Password}"
                //},
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            $"{Port}/tcp",
                            new PortBinding[]
                            {
                                new PortBinding
                                {
                                    HostPort = freePort.ToString(),
                                }
                            }
                        }
                    }
                },
            });

        await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
        await WaitUntilCacheIsAvailableAsync(_containerName, freePort);

        return (container.ID, freePort);
    }

    //public static async Task<string> GetPort()
    //{
    //    var dockerClient = GetDockerClient();
    //    var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
    //    //var existingContainer = await containers.Where(c => c.Names.Any(n => n.Contains(MONGODB_CONTAINER_NAME))).FirstOrDefault;
    //    //return existingContainer.Port;

    //    throw new NotImplementedException();
    //}

    private static bool IsRunningOnWindows()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    private static DockerClient GetDockerClient()
    {
        var dockerUri = IsRunningOnWindows() ? "npipe://./pipe/docker_engine" : "unix:///var/run/docker.sock";
        return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
    }

    private async Task CleanupRunningContainers()
    {
        var dockerClient = GetDockerClient();

        var runningContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });

        foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(_containerName))))
        {
            try
            {
                await EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
            }
            catch
            {
                // Ignoring failures to stop running containers
            }
        }
    }

    public async override Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Containers.StopContainerAsync(dockerContainerId, new ContainerStopParameters());
        await dockerClient.Containers.RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters() { Force = true, RemoveVolumes = true });
    }

    public async override Task EnsureDockerVolumesRemovedAsync(string volumeName)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Volumes.RemoveAsync(volumeName);
    }

    private async Task WaitUntilCacheIsAvailableAsync(string containerName, int cachePort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                var options = new ConfigurationOptions { EndPoints = { $"localhost:{cachePort}"}, AllowAdmin = true };
                Redis = ConnectionMultiplexer.Connect(options);

                var database = Redis.GetDatabase();
                var pong = await database.PingAsync();

                connectionEstablished = true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                await Task.Delay(500);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to the Redis cache docker instance could not be established within {maxWaitTimeSeconds} seconds.");
        }

        return;
    }

    private static int GetFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }

    //public string ConnectionString()
    //{
    //    return $"localhost:{Port}";
    //}
}