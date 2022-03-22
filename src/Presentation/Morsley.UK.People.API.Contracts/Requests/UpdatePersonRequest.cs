namespace Morsley.UK.People.API.Contracts.Requests;

public record UpdatePersonRequest
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }

    public async static ValueTask<UpdatePersonRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        try
        {
            var potentialId = httpContext.Request.RouteValues.SingleOrDefault(_ => _.Key == "id").Value;

            Guid.TryParse(potentialId?.ToString(), out var id);

            var json = await httpContext.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var body = JsonSerializer.Deserialize<UpdatePersonRequest>(json);

            var request = new UpdatePersonRequest
            {
                Id = id,
                FirstName = body?.FirstName,
                LastName = body?.LastName,
                DateOfBirth = body?.DateOfBirth,
                Gender = body?.Gender,
                Sex = body?.Sex
            };

            return await ValueTask.FromResult<UpdatePersonRequest>(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public static bool TryParse(string value) => true;

}

public static class HttpRequestExtensions
{
    /// <summary>
    /// Retrieve the raw body as a string from the Request.Body stream
    /// </summary>
    /// <param name="request">Request instance to apply to</param>
    /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
    /// <returns></returns>
    public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding)
    {
        //if (encoding == null)
            encoding = Encoding.UTF8;

        using (StreamReader reader = new StreamReader(request.Body, encoding))
            return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Retrieves the raw body as a byte array from the Request.Body stream
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    //public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request)
    //{
    //    using (var ms = new MemoryStream(2048))
    //    {
    //        await request.Body.CopyToAsync(ms);
    //        return ms.ToArray();
    //    }
    //}
}