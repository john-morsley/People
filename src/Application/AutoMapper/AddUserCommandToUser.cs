namespace Users.Application.AutoMapper;

public class AddUserCommandToUser : Profile
{
    public AddUserCommandToUser()
    {
        CreateMap<Users.Application.Commands.AddUserCommand, 
                  Users.Domain.Models.User>().ConstructUsing(_ => new User(_.Id == Guid.Empty ? Guid.NewGuid() : _.Id, _.FirstName, _.LastName));
    }
}
