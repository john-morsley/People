namespace Morsley.UK.People.API.Common.Helpers;

public static class PersonResponseHelper
{
    public static PersonResponse From(Person person, IMapper mapper)
    {
        var response = mapper.Map<PersonResponse>(person);
        return response;
    }

    public static ExpandoObject? ShapePersonWithLinks(PersonResponse personResponse)
    {
        var shapedPerson = personResponse.ShapeData();
        var shapedPersonWithLinks = LinksHelper.AddLinks(shapedPerson!, personResponse.Id);
        return shapedPersonWithLinks;
    }
}