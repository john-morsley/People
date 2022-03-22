using Morsley.UK.People.Test.Infrastructure;

namespace Morsley.UK.People.Test.Fixture
{
    public class DockerTestFixture
    {
        private string? _dockerContainerId;
        private int _dockerContainerPort;

        private readonly string _username;
        private readonly string _password;

        public DockerTestFixture(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task RunBeforeTests()
        {
            (_dockerContainerId, var dockerContainerPort) = 
                await MongoDBInDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync(_username, _password);

            _dockerContainerPort = Convert.ToInt32(dockerContainerPort);
        }

        public async Task RunAfterTests()
        {
            await MongoDBInDocker.EnsureDockerContainersStoppedAndRemovedAsync(_dockerContainerId);
        }

        public int GetContainerPort()
        {
            return _dockerContainerPort;
        }

        public string MongoDBConnectionString
        {
            get { return MongoDBInDocker.ConnectionString(_username, _password, _dockerContainerPort); }
        }
    }
}
