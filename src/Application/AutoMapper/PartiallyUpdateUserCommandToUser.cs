namespace Users.Application.AutoMapper;

public class PartiallyUpdateUserCommandToUser : Profile
{
    public PartiallyUpdateUserCommandToUser()
    {
        CreateMap<Users.Application.Commands.PartiallyUpdateUserCommand, Users.Domain.Models.User>();
    }
}
