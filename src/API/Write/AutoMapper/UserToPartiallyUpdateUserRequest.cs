namespace Users.API.Write.AutoMapper
{
    public class UserToPartiallyUpdateUserRequest : Profile
    {
        public UserToPartiallyUpdateUserRequest()
        {
            //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

            //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

            CreateMap<Users.Domain.Models.User, Users.API.Models.Request.v1.PartiallyUpdateUserRequest>();
        }
    }
}