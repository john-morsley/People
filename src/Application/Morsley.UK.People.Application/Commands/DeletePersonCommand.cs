namespace Morsley.UK.People.Application.Commands;

public class DeletePersonCommand : IRequest
{
    public DeletePersonCommand(Guid id)
    {
        Id = id;
    }
        
    public Guid Id { get; set; }
}