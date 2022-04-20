namespace Morsley.UK.People.API.Example.MVC.Controllers;

public class PeopleController : BaseController
{
    public PeopleController(ILogger logger, IConfiguration configuration) : base(logger, configuration)
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        var getPeople = new GetPeople();

        ViewData["URL"] = $"{Request.Scheme}://{Request.Host}";

        return View(getPeople);
    }

    [HttpPost]
    public async Task<IActionResult> Get(GetPeople getPeople)
    {
        var resource = await GetPeopleAsync(getPeople);

        getPeople.Resource = resource;

        ViewData["URL"] = $"{Request.Scheme}://{Request.Host}";

        return View(getPeople);
    }
}
