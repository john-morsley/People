namespace Morsley.UK.People.API.Common.Helpers;

public class ValidatorHelper
{
    public static bool IsRequestValid<T, TV>(T request, TV validator, out ProblemDetails? problemDetails) 
        where T : class 
        where TV : IValidator<T>
    {
        problemDetails = null;

        var result = validator.Validate(request);

        if (result.IsValid) return true;

        problemDetails = new ProblemDetails
        {
            Title = "Validation error(s) occurred!",
            Instance = $"{typeof(T).FullName}",
            Detail = "See the errors field for details.",
            Type = string.Empty,
            Status = StatusCodes.Status422UnprocessableEntity
        };

        // Key: Property, Value: List of problems with that property
        var problems = new Dictionary<string, List<string>>();
        foreach (var error in result.Errors)
        {
            var key = error.PropertyName;
            var value = error.ErrorMessage;
            if (!problems.ContainsKey(key))
            {
                problems.Add(key, new List<string> { value });
            }
            else
            {
                var problem = problems[key];
                problem.Add(value);
            }
        }

        foreach (var problem in problems)
        {
            var key = problem.Key;
            var value = string.Join(" | ", problem.Value);
            problemDetails.Extensions.Add(key, value);
        }

        return false;
    }
}