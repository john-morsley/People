namespace Morsley.UK.People.API.Example.Console;

public class Security
{
    private const string LoginURL = "https://localhost:5002/api/login";

    public async static Task<bool> AuthenticateAsync(HttpClient client, string username, string password)
    {
        System.Console.Write("Authenticating... ");
        var token = await GetJwtTokenAsync(client, username, password);
        if (token is null)
        {
            System.Console.WriteLine("Failed to Authenticate!");
            return false;
        }
        client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        System.Console.WriteLine("Authenticated.");

        return true;
    }

    protected async static Task<string?> GetJwtTokenAsync(HttpClient client, string username, string password)
    {
        var request = new LoginRequest(username, password);
        var result = await client.PostAsJsonAsync(LoginURL, request);
        if (!result.IsSuccessStatusCode) return null;
        if (result.StatusCode != HttpStatusCode.OK) return null;
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
        if (response is null) return null;
        return response.Token;
    }
}
