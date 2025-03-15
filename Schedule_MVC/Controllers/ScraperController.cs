using App.Models;
using App.WebScrapers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Schedule_MVC.Controllers;

public class ScraperController : Controller
{
    private string _json;
    private List<string> _links;

    public ScraperController()
    {
        _json = System.IO.File.ReadAllText("links.json");
        _links = JsonConvert.DeserializeObject<List<string>>(_json);
    }

    public async Task<IActionResult> Index()
    {
        App.Controllers.ScraperController sc = new App.Controllers.ScraperController();
        List<Tutor> allTutors = await sc.Scrape();

        // Przekazujemy dane do widoku
        return View(allTutors);
    }
}
