namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PartiallyUpdateUserRequestToAddUserRequest : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PartiallyUpdateUserRequestToAddUserRequest()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.PartiallyUpdateUserRequest, Users.API.Models.Request.v1.AddUserRequest>();
    }
}
