namespace Morsley.UK.People.Application.Handlers;

public sealed class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository;

    public DeletePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<Unit> Handle(DeletePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        await _personRepository.DeleteAsync(command.Id);
        //var numberOfRowsAffected = await _personRepository.CompleteAsync();
        // ToDo --> Log!

        return Unit.Value;
    }
}