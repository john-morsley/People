namespace Morsley.UK.People.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class GetPeopleRequestToGetPeopleQuery : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public GetPeopleRequestToGetPeopleQuery()
    {
        CreateMap<GetPeopleRequest, GetPeopleQuery>();
    }
}