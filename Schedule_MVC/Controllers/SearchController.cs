using Microsoft.AspNetCore.Mvc;
using Schedule_MVC.Models;

namespace Schedule_MVC.Controllers;

public class SearchController : Controller
{
    private readonly AppDbContext _context;
    public SearchController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var allTutors = _context.ScrapedData.GroupBy(x => x.CourseFullName)
                                            .Select(r => r.First())
                                            .ToList();
        return View(allTutors);
    }
    public IActionResult FilteredSearch(int? id)
    {
        if(id == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var course = _context.ScrapedData.FirstOrDefault(x => x.ID == id);
        if (course != null)
        {
            return View(course);
        }
        else { return NotFound(); }
    }
}
