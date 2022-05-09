namespace Morsley.UK.People.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheable, IRequest<TResponse> where TResponse : class
{
    private readonly ICache _cache;
    private readonly ActivitySource _source;

    public CachingBehavior(
        ICache cache,
        ActivitySource source)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        var name = $"CachingBehavior->{nameof(Handle)}";
        using var activity = _source.StartActivity(name, ActivityKind.Server);

        var key = request.CacheKey;

        activity?.AddTag("CacheKey", key);

        var cached = await _cache.GetValueAsync(key);
        if (cached != null)
        {
            activity?.AddTag("IsCacheHit", true);
            var deserialized = DeserializeResponse(cached);
            if (deserialized is not null) return deserialized;
        }
        activity?.AddTag("IsCacheHit", false);

        var response = await next();

        var serialized = SerializeResponse(response);
        await _cache.SetValueAsync(key, serialized);

        return response;
    }

    private TResponse? DeserializeResponse(string json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new PagedListOfPersonJsonConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var response = JsonSerializer.Deserialize<TResponse>(json, options);

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            var name = $"CacheingBehavior->{nameof(DeserializeResponse)}";
            using var activity = _source.StartActivity(name, ActivityKind.Server);

            // ToDo --> How do we best record this error?
        }

        return null;
    }

    private string? SerializeResponse(TResponse response)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new PagedListOfPersonJsonConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var serialized = JsonSerializer.Serialize(response, options);

            return serialized;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            var name = $"CacheingBehavior->{nameof(SerializeResponse)}";
            using var activity = _source.StartActivity(name, ActivityKind.Server);

            // ToDo --> How do we best record this error?
        }

        return null;
    }
}