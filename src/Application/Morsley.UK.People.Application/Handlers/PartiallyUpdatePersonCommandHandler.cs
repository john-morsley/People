namespace Morsley.UK.People.Application.Handlers
{
    public sealed class PartiallyUpdatePersonCommandHandler : IRequestHandler<PartiallyUpdatePersonCommand, Person>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public PartiallyUpdatePersonCommandHandler(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Person> Handle(PartiallyUpdatePersonCommand command, CancellationToken ct)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var person = _mapper.Map<Person>(command);

            await _personRepository.UpdateAsync(person);
            //var numberOfRowsAffected = await _personRepository.CompleteAsync();
            // ToDo --> Log!

            return person;
        }
    }
}
