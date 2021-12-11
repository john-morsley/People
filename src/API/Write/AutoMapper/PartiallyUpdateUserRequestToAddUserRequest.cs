namespace Users.API.Write.AutoMapper;

public class PartiallyUpdateUserRequestToAddUserRequest : Profile
{
    public PartiallyUpdateUserRequestToAddUserRequest()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.PartiallyUpdateUserRequest, Users.API.Models.Request.v1.AddUserRequest>();
    }
}
