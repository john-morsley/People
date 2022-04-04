namespace Morsley.UK.People.Messaging.IoC;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigureMessaging(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
    {
        if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

        var section = configuration.GetSection(nameof(RabbitMQSettings));
        var rabbitSettings = section.Get<RabbitMQSettings>();
        if (rabbitSettings == null) throw new ArgumentNullException(nameof(rabbitSettings));
        if (!rabbitSettings.IsValid()) throw new ArgumentOutOfRangeException(nameof(rabbitSettings));

        return applicationBuilder;
    }
}