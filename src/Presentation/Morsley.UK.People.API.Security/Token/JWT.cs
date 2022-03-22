namespace Morsley.UK.People.API.Security.Token;

/// <summary>
/// 
/// </summary>
public class JWT
{
    private readonly ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public JWT(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public string GenerateToken(GenerateTokenParameters parameters)
    {
        _logger.Information("Generating a JWT token...");
        try
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, parameters.Username),
                new Claim(ClaimTypes.Email, parameters.Email),
                new Claim(ClaimTypes.GivenName, parameters.FirstName),
                new Claim(ClaimTypes.Surname, parameters.LastName),
                new Claim(ClaimTypes.Role, parameters.Role)
            };

            var notBefore = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddMinutes(30);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(parameters.Key));
            const string algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);
            
            var jwtSecurityToken = new JwtSecurityToken(parameters.Issuer, parameters.Audience, claims, notBefore, expires, signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            _logger.Information("JWT token generated: {0}", token);

            return token;
        }
        catch (Exception e)
        {
            _logger.Error("An unexpected error occurred generating JWT token!", e);
            throw;
        }
    }
}
