namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UserToUserResponse : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UserToUserResponse()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<DateTime?, string?>().ConvertUsing(new DateTimeToStringConverter());

        CreateMap<Users.Domain.Models.User, Users.API.Models.Response.v1.UserResponse>();
    }
}
