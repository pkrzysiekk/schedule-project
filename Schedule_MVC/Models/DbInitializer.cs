using App.Models;

namespace Schedule_MVC.Models;

public class DbInitializer
{
    private static async Task<List<Tutor>> GetAll()
    {
        App.Controllers.ScraperController sc = new App.Controllers.ScraperController();
        List<Tutor> allTutors = await sc.Scrape();
        return allTutors;
    }
    public static async Task Initialize(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        //sprawdzamy, czy w bazie są już jakieś rekordy
        if (context.ScrapedData.Any())
        {
            return;
        }
        var tutors = await GetAll();

        var scrapedDataList = tutors.Select(t => new ScrapedData
        {
            Name = t.Name,
            CourseFullName = t.Course.courseFullName,
            CourseType = t.Course.type,
            IsLead = t.IsLead
        }).ToList();
        foreach (var tutor in scrapedDataList)
        {
            context.ScrapedData.Add(tutor);
        }
        await context.SaveChangesAsync();
    }
}
