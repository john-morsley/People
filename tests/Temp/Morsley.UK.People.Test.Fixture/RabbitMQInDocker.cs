namespace Morsley.UK.People.Test.Fixture;

public class RabbitMQInDocker : InDocker
{
    public const string RABBITMQ_IMAGE = "rabbitmq";
    public const string RABBITMQ_IMAGE_TAG = "3";
    public const string RABBITMQ_CONTAINER_NAME = "IntegrationTesting_RabbitMQ";
    //public const string RABBITMQ_AUTHENTICATION_MECHANISM = "SCRAM-SHA-1";
    //public const int RABBITMQ_PORT = 5672;

    public RabbitMQInDocker(string username, string password, int port) : base(username, password, port) { }

    public async override Task<(string containerId, int port)> EnsureDockerStartedAndGetContainerIdAndPortAsync()
    {
        await CleanupRunningContainers();

        var dockerClient = GetDockerClient();
        var freePort = GetFreePort();

        // This call ensures that the latest Docker image is pulled
        var imagesCreateParameters = new ImagesCreateParameters { FromImage = $"{RABBITMQ_IMAGE}:{RABBITMQ_IMAGE_TAG}" };
        await dockerClient.Images.CreateImageAsync(imagesCreateParameters, null, new Progress<JSONMessage>());

        var container = await dockerClient
            .Containers
            .CreateContainerAsync(new CreateContainerParameters
            {
                Name = RABBITMQ_CONTAINER_NAME,
                Image = $"{RABBITMQ_IMAGE}:{RABBITMQ_IMAGE_TAG}",
                Env = new List<string>
                {
                    $"RABBITMQ_DEFAULT_USER={Username}",
                    $"RABBITMQ_DEFAULT_PASS={Password}"
                },
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
        //await WaitUntilBusAvailableAsync(Username, Password, freePort);

        return (container.ID, freePort);
    }

    //public static async Task<string> GetPort()
    //{
    //    var dockerClient = GetDockerClient();
    //    var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
    //    //var existingContainer = await containers.Where(c => c.Names.Any(n => n.Contains(RABBITMQ_CONTAINER_NAME))).FirstOrDefault;
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

        foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(RABBITMQ_CONTAINER_NAME))))
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

    private async static Task WaitUntilBusAvailableAsync(string username, string password, int busPort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                //var internalIdentity = new MongoInternalIdentity("admin", username);
    //            var passwordEvidence = new PasswordEvidence(password);
    //            //var mongoCredential = new MongoCredential(MONGODB_AUTHENTICATION_MECHANISM, internalIdentity, passwordEvidence);

    //            var mongoClientSettings = new MongoClientSettings
    //            {
    //                Credential = mongoCredential,
    //                Server = new MongoServerAddress("localhost", databasePort)
    //            };

    //            var client = new MongoClient(mongoClientSettings);
    //            var instance = client.GetDatabase(MONGODB_CONTAINER_NAME);

                var factory = new ConnectionFactory();

                factory.UserName = username;
                factory.Password = password;
                factory.HostName = "localhost";
                factory.Port = busPort;

                var connection = factory.CreateConnection();

                

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
            throw new Exception($"Connection to the SQL docker database could not be established within {maxWaitTimeSeconds} seconds.");
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

    public string ConnectionString()
    {
        return $"amqp://{Username}:{Password}@localhost:{Port}";
    }
}