using AutoMapper;

//using Morsley.UK.YearPlanner.Users.API.Converters;

namespace Users.API.Write.AutoMapper
{
    public class PartiallyUpsertUserRequestToPartiallyUpdateUserCommand : Profile
    {
        public PartiallyUpsertUserRequestToPartiallyUpdateUserCommand()
        {
            //CreateMap<string?, Domain.Enumerations.Sex?>()
            //    .ConvertUsing(new StringSexToEnumSexConverter());

            //CreateMap<string?, Domain.Enumerations.Title?>()
            //    .ConvertUsing(new StringTitleToEnumTitleConverter());

            CreateMap<Users.API.Models.Request.v1.PartiallyUpsertUserRequest, 
                      Users.Application.Commands.PartiallyUpdateUserCommand>();
        }
    }
}