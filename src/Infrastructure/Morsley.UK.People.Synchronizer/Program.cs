using Morsley.UK.People.Messaging;

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

    builder.ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<IEventBus, EventBus>();
        });

    var host = builder.Build();

    var bus = host.Services.GetService<IEventBus>();

    bus.Subscribe<PersonAddedEvent, PersonAddedEventHandler>();

    await host.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}