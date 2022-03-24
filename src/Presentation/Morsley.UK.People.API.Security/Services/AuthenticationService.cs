namespace Morsley.UK.People.API.Security.Services;

/// <summary>
/// 
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<User?> Login(string username, string password)
    {
        // ToDo --> Retrieve this user from an actual database.
        await Task.Delay(1);

        if (username != "johnmorsley") return null;
        if (password != "P@$$w0rd!") return null;

        var id = Guid.NewGuid();
        var user = new User(
            id,
            "John",
            "Morsley",
            "johnmorsley",
            "john@morsley.uk",
            "admin");

        return user;
    }
}