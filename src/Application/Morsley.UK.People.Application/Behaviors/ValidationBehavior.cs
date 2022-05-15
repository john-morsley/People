namespace Morsley.UK.People.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(ILogger logger, IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        // Pre...
        Validate(request);

        var response = next();

        // Post...

        return response;
    }

    private void Validate(TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);
        var results = _validators.Select(_ => _.Validate(context));

        var failures = new List<ValidationFailure>();
        foreach (var result in results)
        {
            if (!result.IsValid)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Any()) throw new ValidationException(failures);
    }
}