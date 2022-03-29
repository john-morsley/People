namespace Morsley.UK.People.Application.Events;

public abstract class Event
{
    public string TypeName => this.GetType().Name;
}
