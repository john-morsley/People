namespace Users.Application.AutoMapper;

public class AddUserCommandToUser : Profile
{
    public AddUserCommandToUser()
    {
        CreateMap<Users.Application.Commands.AddUserCommand, Users.Domain.Models.User>();
    }
}
