namespace Morsley.UK.People.API.Security.Models;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Username"></param>
/// <param name="Email"></param>
/// <param name="Role"></param>
public record User(
    Guid Id,
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Role);
