namespace Morsley.UK.People.API.Read.Services;

internal class PersonService
{
    internal async static Task<PersonResponse?> TryGetPerson(
        GetPersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            var getPersonQuery = mapper.Map<GetPersonQuery>(request);
            var person = await mediator.Send(getPersonQuery);
            var personResponse = mapper.Map<PersonResponse>(person);
            return personResponse;

        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to get a person!");
            return null;
        }
    }
}