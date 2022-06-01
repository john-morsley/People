namespace Morsley.UK.People.Application.Handlers;

public sealed class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IEventBus _bus;
    private readonly ICache? _cache;

    public DeletePersonCommandHandler(IPersonRepository personRepository, IEventBus bus)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public DeletePersonCommandHandler(IPersonRepository personRepository, IEventBus bus, ICache cache)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Unit> Handle(DeletePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        await DeletePersonFromDatabase(command.Id);
        await DeletePersonFromCache(command.Id);

        RaisePersonDeletedEvent(command.Id);

        return Unit.Value;
    }

    private async Task DeletePersonFromCache(Guid personId)
    {
        var key = Person.GetCacheKey(personId);
        await _cache.DeleteValueAsync(key);
    }

    private async Task DeletePersonFromDatabase(Guid personId)
    {
        await _personRepository.DeleteAsync(personId);
    }

    private void RaisePersonDeletedEvent(Guid id)
    {
        var personDeletedEvent = new PersonDeletedEvent { Id = id };
        _bus.Publish(personDeletedEvent);
    }
}