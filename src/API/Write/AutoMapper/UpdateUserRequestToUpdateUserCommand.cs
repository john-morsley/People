﻿namespace Users.API.Write.AutoMapper;

public class UpdateUserRequestToUpdateUserCommand : Profile
{
    public UpdateUserRequestToUpdateUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.UpdateUserRequest,
                  Users.Application.Commands.UpdateUserCommand>();
    }
}
