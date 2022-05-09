namespace Morsley.UK.People.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;
    private readonly ActivitySource _source;

    public LoggingBehavior(ILogger logger, ActivitySource source)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        var name = $"{nameof(LoggingBehavior<TRequest, TResponse>)}->{nameof(Handle)}";
        _logger.Debug(name);
        using var activity = _source.StartActivity(name, ActivityKind.Server);

        activity?.AddTag("RequestType", request.GetType().FullName);
        activity?.AddTag("RequestData", request.ToString());

        var response = next();

        return response;
    }
}