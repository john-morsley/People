Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Verbose()
   .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

try
{
    Log.Information("Starting SYNCHRONIZER host...");

    var builder = Host.CreateDefaultBuilder(args);

    builder.UseSerilog((context, services, configuration) => configuration
           .ReadFrom.Configuration(context.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext());

    builder.ConfigureServices((hostContext, services) => {
        var configuration = hostContext.Configuration;
        services.AddHostedService<Worker>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddPersistence(configuration, "ReadMongoDBSettings");
        services.AddApplication();
        services.AddSingleton<PersonAddedEventHandler>();
    });

    var host = builder.Build();

    var bus = host.Services.GetService<IEventBus>();
    if (bus == null) throw new InvalidOperationException("Could not get a handle on the queue!");

    bus.Subscribe<PersonAddedEvent, PersonAddedEventHandler>();

    await host.RunAsync();
}
catch (Exception e)
{
    Log.Error(e, "An unexpected error occurred!");
    Console.WriteLine("The SYNCHRONIZER application has exited because on an unexpected error!");
}

#region System Testing

/// <summary>
/// Required for System Testing only!
/// </summary>
public partial class SynchronizerProgram { }

#endregion System Testing