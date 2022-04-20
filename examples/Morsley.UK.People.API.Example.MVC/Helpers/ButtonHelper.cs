namespace Morsley.UK.People.API.Example.MVC.Helpers;

public class ButtonHelper
{
    public static string GetBootstrapColour(string method)
    {
        if (string.IsNullOrWhiteSpace(method)) return string.Empty;

        switch (method.ToUpper())
        {
            case "GET": return "info";
            case "CREATE": return "success";
            case "PUT": return "warning";
            case "DELETE": return "danger";
            default: return "muted";
        }
    }

    public static string GetHypertextReference(string url, string method, string apiHypertextReference)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));
        if (string.IsNullOrWhiteSpace(apiHypertextReference)) throw new ArgumentNullException(nameof(apiHypertextReference));

        method = method.ToLower();

        var parts = apiHypertextReference.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int length;
        string id;
        string controller;

        switch (method)
        {
            case "get":

                // Expect 'apiHypertextReference' to be:
                // /api/person/[id], or
                // /api/people/[id]

                length = parts.Length;
                controller = parts[length - 2];
                id = parts[length - 1];
                var get = $"{url}/{controller}/get/{id}";
                return get;

            case "delete":

                // Expect 'apiHypertextReference' to be:
                // /api/person/[id], or

                length = parts.Length;
                id = parts[length - 1];
                var delete = $"{url}/person/delete/{id}";
                return delete;

            case "patch":
                throw new NotImplementedException();

            case "post":
                throw new NotImplementedException();

            case "put":

                // Expect 'apiHypertextReference' to be:
                // /api/person/[id], or

                length = parts.Length;
                id = parts[length - 1];
                var update = $"{url}/person/update/{id}";
                return update;

            default: throw new NotImplementedException();
        }
    }
}