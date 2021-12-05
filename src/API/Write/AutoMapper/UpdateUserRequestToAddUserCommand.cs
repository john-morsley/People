namespace Users.API.Write.AutoMapper;

public class UpdateUserRequestToAddUserCommand : Profile
{
    public UpdateUserRequestToAddUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.UpdateUserRequest,
                  Users.Application.Commands.AddUserCommand>();
    }
}
