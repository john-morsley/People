namespace Morsley.UK.People.Messaging.Configuration;

public class RabbitMQSettings
{
    public string? Host { get; set; }

    public string? Port { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    //public string? QueueName { get; set; }

    public bool IsValid()
    {
        if (Host is null) return false;
        if (Port is null) return false;
        if (!int.TryParse(Port, out var port)) return false;
        if (port <= 1024) return false;
        if (port >= 65535) return false;
        if (Username is null) return false;
        if (Password is null) return false;
        return true;
    }

    public override string ToString()
    {
        var host = GetDisplayValue(Host);
        var port = GetDisplayValue(Port);
        var username = GetDisplayValue(Username);
        var password = GetDisplayValue(Password);
        return $"Host: '{host}' | Port: '{port}' | Username: '{username}' | Password: '{password}'";
    }

    private string GetDisplayValue(string? value)
    {
        if (value is null) return "[NULL]";
        if (value.Length == 0) return "[EMPTY]";
        return value;
    }
}
