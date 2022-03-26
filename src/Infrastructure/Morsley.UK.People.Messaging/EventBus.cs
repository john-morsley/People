namespace Morsley.UK.People.Messaging;

public class EventBus : IEventBus
{
    public EventBus()
    {
        
    }

    public void Publish<T>(T @event) where T : Event
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        throw new NotImplementedException();
    }
}
