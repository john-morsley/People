namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class GetPersonRequestToGetPersonQuery : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public GetPersonRequestToGetPersonQuery()
    {
        CreateMap<GetPersonRequest, GetPersonQuery>();
    }
}
