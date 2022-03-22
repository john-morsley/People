namespace Morsley.UK.People.API.Security.Token;

/// <summary>
/// 
/// </summary>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Username"></param>
/// <param name="Email"></param>
/// <param name="Role"></param>
/// <param name="Key"></param>
/// <param name="Issuer"></param>
/// <param name="Audience"></param>
/// <param name="TimeToLiveInMintes"></param>
public record GenerateTokenParameters
(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Role,
    string Key,
    string Issuer,
    string Audience, 
    int TimeToLiveInMintes = 30
);