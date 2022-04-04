namespace Morsley.UK.People.Persistence.Configuration;

public  class MongoDBSettings
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
        var host = GetDisplayValue(Host);
        var port = GetDisplayValue(Port);
        var username = GetDisplayValue(Username);
        var password = GetDisplayValue(Password);
        var databaseName = GetDisplayValue(DatabaseName);
        var tableName = GetDisplayValue(TableName);
        return $"Host: '{host}' | Port: '{port}' | Username: '{username}' | Password: '{password}' | DatabaseName: '{databaseName}' | TableName: '{tableName}'";
    }

    private string GetDisplayValue(string? value)
    {
        if (value is null) return "[NULL]";
        if (value.Length == 0) return "[EMPTY]";
        return value;
    }

}