namespace Morsley.UK.People.API.Security.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    User? Login(string username, string password);
}
