namespace Morsley.UK.People.API.Example.MVC.Controllers;

public class PersonController : BaseController
{
    public PersonController(ILogger logger, IConfiguration configuration) : base(logger, configuration)
    {
    }

    [HttpGet]
    public IActionResult Create()
    {
        var person = new Person();
        person.FirstName = $"FirstName-{Guid.NewGuid()}";
        person.LastName = $"LastName-{Guid.NewGuid()}";
        return View(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Person person)
    {
        if (!ModelState.IsValid) return View(person);

        var resource = await CreatePersonAsync(person);

        if (resource is null)
        {
            // ToDo --> Status message: "Could not create person"?
            return RedirectToAction("Index", "Home");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var json = JsonSerializer.Serialize(resource, options);

        TempData["CreatedPersonResource"] = json;
        return RedirectToAction(controllerName: "Person", actionName: "Created");
    }

    [HttpGet]
    public IActionResult Created()
    {
        var json = TempData["CreatedPersonResource"]?.ToString();

        if (json is null)
        {
            // ToDo --> Status message: "Could not create person"?
            return RedirectToAction("Index", "Home");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var resource = JsonSerializer.Deserialize<PersonResource>(json, options);

        ViewData["URL"] = $"{Request.Scheme}://{Request.Host}";

        return View(resource);
    }
    
    [HttpGet]
    public IActionResult Delete(Guid? id)
    {
        var path = TempData[$"DELETE_{id}"];

        return View(path);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new InvalidOperationException("Did not expect 'path' to be empty!");

        await DeletePersonAsync(path);

        return View(model: path);
    }

    [HttpGet]
    public IActionResult Update()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (id is null) return View();

        var url = TempData[$"GET_{id}"]?.ToString();

        if (url is null) throw new InvalidOperationException("Did not expect 'url' to be null here!");

        var resource = await GetPersonAsync(url);

        ViewData["URL"] = $"{Request.Scheme}://{Request.Host}";

        return View(resource);
    }
}
