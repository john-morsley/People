const int MaximumNumberOfSecondsToRetries = 100;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Verbose()
   .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

var numberOfSecondsToRetries = 0;
do
{
    try
    {
        Log.Debug("Starting SYNCHRONIZER...");

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;

        var builder = Host.CreateDefaultBuilder(args);

        builder.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        builder.ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            var source = new ActivitySource("Morsley.UK.People.Synchronizer", "0.1.0");
            services.AddHostedService<Worker>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddPersistence(configuration, "ReadMongoDBSettings");
            services.AddApplication();
            services.AddSingleton<ICache, NoCache>();
            services.AddSingleton<PersonAddedEventHandler>();
            services.AddSingleton<PersonDeletedEventHandler>();
            services.AddSingleton<PersonUpdatedEventHandler>();
            services.AddSingleton(source);
        });

        var host = builder.Build();

        var bus = host.Services.GetService<IEventBus>();
        if (bus == null) throw new InvalidOperationException("Could not get a handle on the queue!");

        bus.Subscribe<PersonAddedEvent, PersonAddedEventHandler>();
        bus.Subscribe<PersonDeletedEvent, PersonDeletedEventHandler>();
        bus.Subscribe<PersonUpdatedEvent, PersonUpdatedEventHandler>();

        await host.RunAsync();
    }
    catch
    {
        Log.Warning("Encountered an unexpected error! Is the infrastructure ready?");
    }
    finally
    {
        await Task.Delay(1000);
    }

    Log.Debug($"Retrying... Number of retries = {numberOfSecondsToRetries}");

    numberOfSecondsToRetries++;
}
while (numberOfSecondsToRetries <= MaximumNumberOfSecondsToRetries);

Log.Warning("Giving up! :-(");

#region System Testing

/// <summary>
/// Required for System Testing only!
/// </summary>
public partial class SynchronizerProgram { }

#endregion System Testing