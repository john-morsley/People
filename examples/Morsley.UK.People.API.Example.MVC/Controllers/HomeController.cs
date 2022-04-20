namespace Morsley.UK.People.API.Example.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger _logger;

    public HomeController(ILogger logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    //public IActionResult Add()
    //{
    //    return View();
    //}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}