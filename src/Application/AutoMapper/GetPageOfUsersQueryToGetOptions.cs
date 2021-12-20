namespace Users.Application.AutoMapper;

public class GetPageOfUsersQueryToGetOptions : Profile
{
    public GetPageOfUsersQueryToGetOptions()
    {
        //CreateMap<string, IGetOptions>().ConvertUsing(new StringOrderByToGetOptions);


        CreateMap<Application.Queries.GetPageOfUsersQuery, Application.Models.GetOptions>().ConvertUsing(new GetPageOfUsersQueryToGetOptionsConverter());
    }
}
