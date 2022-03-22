namespace Morsley.UK.People.API.Contracts.Requests;

public record PartiallyUpdatePersonRequest
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }

//    public async static ValueTask<PartiallyUpdatePersonRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
//    {
//        try
//        {
//            var potentialId = httpContext.Request.RouteValues.SingleOrDefault(_ => _.Key == "id").Value;

//            Guid.TryParse(potentialId?.ToString(), out var id);

//            //var json = await httpContext.Request.GetRawBodyStringAsync(Encoding.UTF8);
//            //var body = JsonSerializer.Deserialize<UpdatePersonRequest>(json);

//            var request = new PartiallyUpdatePersonRequest
//            {
//                Id = id,
//                //FirstName = body?.FirstName,
//                //LastName = body?.LastName,
//                //DateOfBirth = body?.DateOfBirth,
//                //Gender = body?.Gender,
//                //Sex = body?.Sex
//            };

//            return await ValueTask.FromResult<PartiallyUpdatePersonRequest>(request);
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e);
//            return null;
//        }
//    }
}