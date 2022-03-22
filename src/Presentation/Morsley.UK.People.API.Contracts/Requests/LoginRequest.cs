namespace Morsley.UK.People.API.Contracts.Requests;

public record LoginRequest(
    string Username,
    string Password);