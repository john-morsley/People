namespace Morsley.UK.People.API.Common.Helpers;

public class PersonHelper
{
    public async static Task<Person> AddPerson(
        Guid id, 
        AddPersonRequest addPersonRequest, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger,
        ActivitySource source)
    {
        var name = $"{nameof(PersonHelper)}->{nameof(AddPerson)}";
        logger.Debug(name);
        using var activity = source.StartActivity(name, ActivityKind.Server);

        var addPersonCommand = mapper.Map<AddPersonCommand>(addPersonRequest);
        addPersonCommand.Id = id;
        var person = await mediator.Send(addPersonCommand);
        return person;
    }

    public async static Task<Person> AddPerson(
        AddPersonRequest addPersonRequest,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        var name = $"{nameof(PersonHelper)}->{nameof(AddPerson)}";
        logger.Debug(name);
        using var activity = source.StartActivity(name, ActivityKind.Server);

        var addPersonCommand = mapper.Map<AddPersonCommand>(addPersonRequest);
        var person = await mediator.Send(addPersonCommand);
        return person;
    }

    public static Task<Person> AddPerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        var name = $"{nameof(PersonHelper)}->{nameof(AddPerson)}";
        logger.Debug(name);
        using var activity = source.StartActivity(name, ActivityKind.Server);

        var addPersonRequest = mapper.Map<AddPersonRequest>(request);
        return AddPerson(request.Id, addPersonRequest, mapper, mediator, logger, source);
    }

    public async static Task DeletePerson(
        Guid id,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            var name = $"{nameof(PersonHelper)}->{nameof(DeletePerson)}";
            logger.Debug(name);
            using var activity = source.StartActivity(name, ActivityKind.Server);

            var deletePersonCommand = new DeletePersonCommand(id);
            await mediator.Send(deletePersonCommand);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async static Task<bool> DoesPersonExist(
        Guid personId,
        IMediator mediator,
        ILogger logger)
    {
        logger.Debug("DoesPersonExist");
        var personExistsQuery = new PersonExistsQuery(personId);
        var doesPersonExist = await mediator.Send(personExistsQuery);
        return doesPersonExist;
    }

    public async static Task<Person> GetPerson(
        Guid id,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        logger.Debug("GetPerson");
        var getPersonRequest = new GetPersonRequest { Id = id };
        var getPersonQuery = mapper.Map<GetPersonQuery>(getPersonRequest);
        var person = await mediator.Send(getPersonQuery);
        return person;
    }

    public async static Task<Person> UpdatePerson(
        UpdatePersonRequest request, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger)
    {
        logger.Debug("UpdatePerson");
        var partiallyUpdatePersonCommand = mapper.Map<UpdatePersonCommand>(request);
        var updatedPerson = await mediator.Send(partiallyUpdatePersonCommand);
        return updatedPerson;
    }
}