using Microsoft.AspNetCore.Mvc;
using Schedule_MVC.Models;
using System.Diagnostics;

namespace Schedule_MVC.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> UpdateDatabase()
    {
        DbUpdate db = new DbUpdate(_context);
        await db.ClearDb();
        await DbInitializer.Initialize(_context);

        return View(nameof(Index));
    }
}