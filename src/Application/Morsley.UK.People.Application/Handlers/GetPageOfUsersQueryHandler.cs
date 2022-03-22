using Morsley.UK.People.Application.Models;

namespace Morsley.UK.People.Application.Handlers
{
    public sealed class GetPageOfPeopleQueryHandler : IRequestHandler<GetPeopleQuery, IPagedList<Person>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public GetPageOfPeopleQueryHandler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IPagedList<Person>> Handle(GetPeopleQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var getOptions = _mapper.Map<GetOptions>(query);

            var people = await _personRepository.GetPageAsync(getOptions);

            return people;
        }
    }
}
