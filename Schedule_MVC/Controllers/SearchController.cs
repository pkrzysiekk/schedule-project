using Microsoft.AspNetCore.Mvc;

namespace Schedule_MVC.Controllers;

public class SearchController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
