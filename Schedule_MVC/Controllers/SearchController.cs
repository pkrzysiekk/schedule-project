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
    public IActionResult FilteredSearch(string? name)
    {
        if(name == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var course = _context.ScrapedData.Where(x => x.CourseFullName == name && x.IsLead == true).ToList();
        if (course.Count()!=0)
        {
            return View(course);
        }
        else
        {
            course = _context.ScrapedData.Where(x => x.CourseFullName == name).ToList();
            return View(course);
        }
    }
}
