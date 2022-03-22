namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PersistencePagedListToApiPagedList : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PersistencePagedListToApiPagedList()
    {
        CreateMap<Person, PersonResponse>();

        CreateMap<PagedList<Person>, PagedList<PersonResponse>>().ConvertUsing<PagedListTypeConverter>();
    }
}
