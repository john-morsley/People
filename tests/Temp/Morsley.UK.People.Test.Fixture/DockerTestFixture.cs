namespace Morsley.UK.People.Test.Fixture;

public class DockerTestFixture<T> where T : InDocker
{
    public readonly InDocker InDocker;

    public DockerTestFixture(string username, string password, int port)
    {
        InDocker = (T)Activator.CreateInstance(typeof(T), username, password, port);
    }

    public async Task RunBeforeTests()
    {
        (string dockerContainerId, int dockerContainerPort) = await InDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync();

        InDocker.Port = Convert.ToInt32(dockerContainerPort);
        InDocker.ContainerId = dockerContainerId;
    }

    public async Task RunAfterTests()
    {
        await InDocker.EnsureDockerContainersStoppedAndRemovedAsync(InDocker.ContainerId);
    }

    public int GetContainerPort()
    {
        return InDocker.Port;
    }
}