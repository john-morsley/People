namespace Morsley.UK.People.Test.Fixture.Abstract;

public abstract class InDocker
{
    public string Username { get; }

    public string Password { get; }

    public int Port { get; set; }

    public string ContainerId { get; set; }

    public InDocker(string username, string password, int port)
    {
        Username = username;
        Password = password;
        Port = port;
    }

    public abstract Task<(string containerId, int port)> EnsureDockerStartedAndGetContainerIdAndPortAsync();

    public abstract Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId);
    
    public abstract Task EnsureDockerVolumesRemovedAsync(string volumeName);
}