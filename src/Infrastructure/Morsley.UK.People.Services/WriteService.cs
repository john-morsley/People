using AutoMapper;
using MediatR;
using Morsley.UK.People.Application.Commands;

namespace Morsley.UK.People.Services
{
    public class WriteService : IWriteService
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public WriteService(IMapper mapper, IMediator mediator, ILogger logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public Person? AddPerson()
        {
            var addUserCommand = _mapper.Map<AddPersonCommand>(addUserRequest);
            var user = await _mediator.Send(addUserCommand);
            var userResponse = _mapper.Map<PersonResponse>(user);
            return userResponse;

        }
    }
}