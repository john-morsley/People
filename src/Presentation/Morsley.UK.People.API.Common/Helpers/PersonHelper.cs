namespace Morsley.UK.People.API.Common.Helpers;

public class PersonHelper
{
    public async static Task<Person> AddPerson(
        Guid id, 
        AddPersonRequest addPersonRequest, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger)
    {
        var addPersonCommand = mapper.Map<AddPersonCommand>(addPersonRequest);
        addPersonCommand.Id = id;
        var person = await mediator.Send(addPersonCommand);
        return person;
    }

    public async static Task<Person> AddPerson(
        AddPersonRequest addPersonRequest,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        var addPersonCommand = mapper.Map<AddPersonCommand>(addPersonRequest);
        var person = await mediator.Send(addPersonCommand);
        return person;
    }

    public static Task<Person> AddPerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        var addPersonRequest = mapper.Map<AddPersonRequest>(request);
        return AddPerson(request.Id, addPersonRequest, mapper, mediator, logger);
    }

    public async static Task DeletePerson(
        Guid id,
        IMediator mediator,
        ILogger logger)
    {
        var deletePersonCommand = new DeletePersonCommand(id);
        await mediator.Send(deletePersonCommand);
    }

    public async static Task<bool> DoesPersonExist(
        Guid personId,
        IMediator mediator,
        ILogger logger)
    {
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
        var partiallyUpdatePersonCommand = mapper.Map<UpdatePersonCommand>(request);
        var updatedPerson = await mediator.Send(partiallyUpdatePersonCommand);
        return updatedPerson;
    }
}