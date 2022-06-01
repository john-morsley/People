namespace Morsley.UK.People.API.Example.MVC.Controllers;

public class PersonController : BaseController
{
    public PersonController(ILogger logger, IConfiguration configuration) : base(logger, configuration)
    {
    }

    [HttpGet]
    public IActionResult Create()
    {
        var name = Logging.FormatMessage($"{nameof(PersonController)}-{nameof(Create)}-GET");
        _logger.Debug(name);

        var person = new Person();
        person.FirstName = $"FirstName-{Guid.NewGuid()}";
        person.LastName = $"LastName-{Guid.NewGuid()}";

        return View(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Person person)
    {
        var name = $"{nameof(PersonController)}-{nameof(Create)}-POST";
        _logger.Debug(Logging.FormatMessage(name));

        _logger.Debug(Logging.FormatMessage($"{name} - Person: {person}"));

        if (!ModelState.IsValid)
        {
            _logger.Debug(Logging.FormatMessage($"{name} - Person is not valid"));
            
            return View(person);
        }

        _logger.Debug(Logging.FormatMessage($"{name} - Creating Person..."));

        var resource = await CreatePersonAsync(person);

        if (resource is null)
        {
            _logger.Debug(Logging.FormatMessage($"{name} - Could not create person"));
            // ToDo --> Status message: "Could not create person"?
            return RedirectToAction("Index", "Home");
        }

        _logger.Debug(Logging.FormatMessage($"{name} - Person created"));

        var getLink = resource!.Links!.Single(_ => _.Method == "GET" && _.Relationship == "self");
        if (getLink is null) throw new InvalidOperationException("Expected GET/self link!");

        TempData[$"GET_{resource!.Data!.Id}"] = getLink.HypertextReference;

        return RedirectToAction(controllerName: "Person", actionName: "Get", routeValues: new { id = resource!.Data!.Id });
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
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
    public async Task<IActionResult> Update(Guid id)
    {
        var getPath = TempData[$"GET_{id}"]?.ToString();
        var putPath = TempData[$"PUT_{id}"]?.ToString();
        
        if (getPath is null) throw new InvalidOperationException("Did not expect 'getPath' to be null here!");
        if (putPath is null) throw new InvalidOperationException("Did not expect 'putPath' to be null here!");

        ViewBag.GetPath = getPath;
        ViewBag.PutPath = putPath;

        var person = await GetPersonAsync(getPath);

        var update = new UpdatePerson
        {
            Id = person!.Data!.Id.ToString(),
            FirstName = person!.Data!.FirstName,
            LastName = person!.Data!.LastName,
            Sex = person!.Data!.Sex,
            Gender = person!.Data!.Gender,
            Day = GetDay(person!.Data!.DateOfBirth).ToString(),
            Month = GetMonth(person!.Data!.DateOfBirth).ToString(),
            Year = GetYear(person!.Data!.DateOfBirth).ToString()
            //Email = person!.Data!.Email,
            //Mobile = person!.Data!.Mobile,
        };

        return View(update);
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromForm] UpdatePerson person)
    {
        var personId = person.Id;

        var getPath = TempData[$"GET_{personId}"]?.ToString();
        var putPath = TempData[$"PUT_{personId}"]?.ToString();

        if (getPath is null) throw new InvalidOperationException("Did not expect 'getPath' to be null here!");
        if (putPath is null) throw new InvalidOperationException("Did not expect 'putPath' to be null here!");

        ViewBag.GetPath = getPath;
        ViewBag.PutPath = putPath;

        if (!ModelState.IsValid) return View(person);

        var resource = await UpdatePersonAsync(person, putPath);

        if (resource is null)
        {
            // ToDo --> Status message: "Could not create person"?
            return RedirectToAction("Index", "Home");
        }

        //var getLink = resource!.Links!.Single(_ => _.Method == "GET" && _.Relationship == "self");
        //if (getLink is null) throw new InvalidOperationException("Expected GET/self link!");

        //TempData[$"PUT_{resource!.Data!.Id}"] = getLink.HypertextReference;

        return RedirectToAction(controllerName: "Person", actionName: "Get", routeValues: new { id = resource!.Data!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        var getPath = TempData[$"GET_{id}"]?.ToString();

        if (getPath is null) throw new InvalidOperationException("Did not expect 'getPath' to be null here!");

        ViewBag.GetPath = getPath;

        ViewBag.URL = $"{Request.Scheme}://{Request.Host}";

        if (id is null) return View();

        var resource = await GetPersonAsync(getPath);

        return View(resource);
    }
}
