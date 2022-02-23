namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UserToPartiallyUpdateUserRequest : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UserToPartiallyUpdateUserRequest()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.Domain.Models.User, Users.API.Models.Request.v1.PartiallyUpdateUserRequest>();
    }
}
