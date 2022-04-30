using Serilog;
using Serilog.Events;

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

    builder.Services.AddOpenTelemetryTracing(configure =>
    {
        configure
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion))
            .AddSource(serviceName)
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddMongoDBInstrumentation()
            .AddConsoleExporter()
            .AddZipkinExporter()
            .AddJaegerExporter();
    });

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
finally
{
    Log.CloseAndFlush();
}