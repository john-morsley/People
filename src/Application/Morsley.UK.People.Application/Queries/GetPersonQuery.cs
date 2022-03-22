namespace Morsley.UK.People.Application.Queries
{
    public sealed class GetPersonQuery : IRequest<Person>
    {
        public Guid Id { get; set; }
    }
}
