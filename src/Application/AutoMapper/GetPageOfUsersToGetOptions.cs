namespace Users.Application.AutoMapper;

public class GetUsersQueryToGetOptions : Profile
{
    public GetUsersQueryToGetOptions()
    {
        CreateMap<Application.Queries.GetPageOfUsersQuery, Application.Models.GetOptions>();
    }
}
