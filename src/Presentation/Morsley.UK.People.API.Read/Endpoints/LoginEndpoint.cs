//namespace Morsley.UK.People.API.Read.Endpoints;

///// <summary>
///// 
///// </summary>
//public static class LoginEndpoint
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="application"></param>
//    public static void MapLoginEndpoint(this WebApplication application)
//    {
//        application.MapMethods("/login", new[] { "POST" }, async (
//                    [FromBody] LoginRequest request,
//                    [FromServices] IAuthenticationService service,
//                    [FromServices] IConfiguration configuration, 
//                    [FromServices] ILogger logger)
//                    =>
//                    await Login(request, service, configuration, logger))
//                   .AllowAnonymous()
//                   .Accepts<LoginRequest>("application/json")
//                   .Produces(StatusCodes.Status401Unauthorized)
//                   .Produces<LoginResponse>(StatusCodes.Status200OK, "application/json")
//                   .WithName("Login");
//    }

//    private async static Task<IResult> Login(
//        LoginRequest request, 
//        IAuthenticationService service, 
//        IConfiguration configuration,
//        ILogger logger)
//    {
//        logger.Information("Login endpoint...");

//        var user = await service.Login(request.Username, request.Password);

//        if (user == null)
//        {
//            logger.Information("User is Unauthorized.");
//            return Results.Unauthorized();
//        }

//        logger.Information("User is authorized.");

//        var token = GenerateToken(user, configuration, logger);

//        var response = new LoginResponse(token);

//        return Results.Ok(response);
//    }

//    private static string GenerateToken(User user, IConfiguration configuration, ILogger logger)
//    {
//        var key = configuration["Jwt:Key"];
//        var issuer = configuration["Jwt:Issuer"];
//        var audience = configuration["Jwt:Audience"];

//        var jwt = new JWT(logger);
//        var parameters = new GenerateTokenParameters(
//            user.FirstName,
//            user.LastName,
//            user.Username,
//            user.Email,
//            user.Role,
//            key,
//            issuer,
//            audience);
//        var token = jwt.GenerateToken(parameters);

//        return token;
//    }
//}