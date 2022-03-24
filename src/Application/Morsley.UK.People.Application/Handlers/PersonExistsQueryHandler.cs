namespace Morsley.UK.People.Application.Handlers
{
    public sealed class PersonExistsQueryHandler : IRequestHandler<PersonExistsQuery, bool>
    {
        private readonly IPersonRepository _personRepository;

        public PersonExistsQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<bool> Handle(PersonExistsQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            return await _personRepository.ExistsAsync(query.Id);
        }
    }
}
