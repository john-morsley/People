namespace Users.API.Write.AutoMapper;

public class UpsertUserRequestToUpdateUserCommand : Profile
{
    public UpsertUserRequestToUpdateUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.UpsertUserRequest,
                  Users.Application.Commands.UpdateUserCommand>();
    }
}
