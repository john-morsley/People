//Log.Logger = new LoggerConfiguration()
//   .MinimumLevel.Information()
//   .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//   .Enrich.FromLogContext()
//   .WriteTo.Console()
//   .CreateBootstrapLogger();

//try
//{
//    Log.Information("Starting web host...");

//    var builder = WebApplication.CreateBuilder(args);

//    // Configure Services...

//    builder.Host.UseSerilog((context, services, configuration) => configuration
//           .ReadFrom.Configuration(context.Configuration)
//           .ReadFrom.Services(services)
//           .Enrich.FromLogContext());

//    builder.Services.ConfigureSwaggerWithoutAuthentication();

//    builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
//    builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();

//    var key = Environment.GetEnvironmentVariable(Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable);
//    if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException($"Missing environment variable: {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
//    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> { { "Jwt:Key", key } });

//    builder.Services.AddCors();

//    var executingAssembly = Assembly.GetExecutingAssembly();
//    builder.Services.AddFluentValidation(_ => _.RegisterValidatorsFromAssembly(executingAssembly));

//    // Configure Application...

//    var application = builder.Build();

//    application.UseSerilogRequestLogging(_ => { _.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms"; });

//    application.UseHttpsRedirection();

//    application.ConfigureSwagger();
    
//    application.MapLoginEndpoint();

//    application.UseCors();

//    application.Run();
//}
//catch (Exception e)
//{
//    Log.Fatal(e, "Host terminated unexpectedly!");
//    return 1;
//}
//finally
//{
//    Log.CloseAndFlush();
//}

//return 0;

//#region System Testing

///// <summary>
///// Required for System Testing only!
///// </summary>
//public partial class SecurityProgram { }

//#endregion System Testing