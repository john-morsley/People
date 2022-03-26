namespace Morsley.UK.People.Application.Interfaces;

public interface IEventHandler<in TEvent> : IEventHandler
{
    Task Handle(TEvent @event);
}

public interface IEventHandler
{

}
