namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class DeletePersonRequestToDeletePersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public DeletePersonRequestToDeletePersonCommand()
    {
        CreateMap<DeletePersonRequest, DeletePersonCommand>();
    }
}
