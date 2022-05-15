const string serviceName = "Morsley.UK.People.API.Example.MVC";
const string serviceVersion = "0.1.0";

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Verbose()
   .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

try
{
    Log.Information("Starting Example MVC web host...");

    Activity.DefaultIdFormat = ActivityIdFormat.W3C;

    var builder = WebApplication.CreateBuilder(args);

//   builder.Services.AddLogging(configure => configure.AddSeq());

    builder.Host.UseSerilog((context, services, configuration) => configuration
           .ReadFrom.Configuration(context.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext());

    // Add services to the container.

    builder.Services.AddOpenTelemetryTracing(tpb =>
    {
        tpb.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion))
           .AddSource(serviceName)
           .AddHttpClientInstrumentation()
           .AddAspNetCoreInstrumentation()
           .AddMongoDBInstrumentation()
           .AddZipkinExporter()
           .AddJaegerExporter();
    });

    var source = new ActivitySource(serviceName, serviceVersion);
    builder.Services.AddSingleton(source);

    builder.Services.AddControllersWithViews();

    var application = builder.Build();

    application.UseSerilogRequestLogging(_ => { _.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms"; });

    if (!application.Environment.IsDevelopment())
    {
        application.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        application.UseHsts();
    }

    application.UseHttpsRedirection();
    application.UseStaticFiles();

    application.UseRouting();

    application.UseAuthorization();

    application.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

    application.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;