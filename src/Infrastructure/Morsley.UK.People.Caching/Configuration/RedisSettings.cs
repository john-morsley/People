using Morsley.UK.People.Common.Extensions;

namespace Morsley.UK.People.Persistence.Configuration;

public  class RedisSettings
{
    public string? Host { get; set; }

    public string? Port { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? DatabaseName { get; set; }

    public string? TableName { get; set; }

    public bool IsValid()
    {
        if (Host is null) return false;
        if (Port is null) return false;
        if (!int.TryParse(Port, out var port)) return false;
        if (port <= 1024) return false;
        if (port >= 65535) return false;
        if (Username is null) return false;
        if (Password is null) return false;
        if (DatabaseName is null) return false;
        if (TableName is null) return false;
        return true;
    }

    public override string ToString()
    {
        return $"Host: '{Host.GetDisplayValue()}' | " +
               $"Port: '{Port.GetDisplayValue()}' | " +
               $"Username: '{Username.GetDisplayValue()}' | " +
               $"Password: '{Password.GetDisplayValue()}' | " +
               $"DatabaseName: '{DatabaseName.GetDisplayValue()}' | " +
               $"TableName: '{TableName.GetDisplayValue()}'";
    }
}