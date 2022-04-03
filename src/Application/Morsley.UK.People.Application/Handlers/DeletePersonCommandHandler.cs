namespace Morsley.UK.People.Application.Handlers;

public sealed class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IEventBus _bus;

    public DeletePersonCommandHandler(IPersonRepository personRepository, IEventBus bus)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task<Unit> Handle(DeletePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        await _personRepository.DeleteAsync(command.Id);

        RaisePersonDeletedEvent(command.Id);

        return Unit.Value;
    }

    private void RaisePersonDeletedEvent(Guid id)
    {
        var personDeletedEvent = new PersonDeletedEvent { Id = id };
        _bus.Publish(personDeletedEvent);
    }

}