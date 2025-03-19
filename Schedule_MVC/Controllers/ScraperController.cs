using App.Models;
using App.WebScrapers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Schedule_MVC.Models;

namespace Schedule_MVC.Controllers;

public class ScraperController : Controller
{
    private readonly AppDbContext _context;
    public ScraperController(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var tutors = await _context.ScrapedData.ToListAsync();
        // Przekazujemy dane do widoku
        return View(tutors);
    }
}
