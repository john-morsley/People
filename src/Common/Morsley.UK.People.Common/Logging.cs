namespace Morsley.UK.People.Common;

public class Logging
{
    public static string FormatMessage(string message, int indentations = 20)
    {
        var indentation = new string('-', indentations);
        return $"{indentation} {message} {indentation}";
    }
}
