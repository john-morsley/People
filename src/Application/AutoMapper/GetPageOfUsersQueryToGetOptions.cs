namespace Users.Application.AutoMapper;

public class GetPageOfUsersQueryToGetOptions : Profile
{
    public GetPageOfUsersQueryToGetOptions()
    {
        CreateMap<Application.Queries.GetPageOfUsersQuery, Application.Models.GetOptions>().ConvertUsing(new GetPageOfUsersQueryToGetOptionsConverter());
    }
}
