namespace Users.API.Write.AutoMapper;

public class CreateUserRequestToAddUserCommand : Profile
{
    public CreateUserRequestToAddUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.CreateUserRequest, Users.Application.Commands.AddUserCommand>();
    }
}
