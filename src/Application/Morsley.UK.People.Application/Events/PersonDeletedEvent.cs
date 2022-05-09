namespace Morsley.UK.People.Application.Events;

public class PersonDeletedEvent : Event
{
    public Guid Id { get; set; }

    public override string ToString()
    {
        return $"Id:{Id}";
    }
}
