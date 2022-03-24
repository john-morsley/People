namespace Morsley.UK.People.Application.Handlers
{
    public sealed class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Person>
    {
        private readonly IPersonRepository _personRepository;

        public GetPersonQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Person> Handle(GetPersonQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var person = await _personRepository.GetByIdAsync(query.Id);

            return person;
        }
    }
}
