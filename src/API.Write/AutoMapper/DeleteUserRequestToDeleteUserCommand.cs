using AutoMapper;

namespace Users.API.Write.AutoMapper
{
    public class DeleteUserRequestToDeleteUserCommand : Profile
    {
        public DeleteUserRequestToDeleteUserCommand()
        {
            CreateMap<Users.API.Models.Request.v1.DeleteUserRequest,
                      Users.Application.Commands.DeleteUserCommand>();
        }
    }
}