namespace Users.API.Write.AutoMapper
{
    public class UserToUserResponse : Profile
    {
        public UserToUserResponse()
        {
            //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

            //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

            CreateMap<Users.Domain.Models.User, Users.API.Models.Response.v1.UserResponse>();
        }
    }
}