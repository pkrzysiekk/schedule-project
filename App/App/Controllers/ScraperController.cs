using App.Models;
using App.WebScrapers;
using Newtonsoft.Json;

namespace App.Controllers;

public class ScraperController
{
    private string _json;
    private List<ScheduleLink> _links;

    public ScraperController()
    {
        _json = File.ReadAllText("links.json");
        _links = JsonConvert.DeserializeObject<List<ScheduleLink>>(_json);
    }

    public async Task<List<Tutor>> Scrape()
    {
        List<Task<List<Tutor>>> tasks = new();

        foreach (var link in _links)
        {
            tasks.Add(Task.Run(() =>
            {
                using WebScraper scraper = new WebScraper();
                var result = scraper.StartScraping(link.Url, link.Type);
                scraper.Quit();
                return result;
            }));
        }

        List<Tutor>[] allTutors = await Task.WhenAll(tasks);
        WebScraper.SaveDictionary();

        List<Tutor> formatedTutors = new List<Tutor>();
        formatedTutors.AddRange(allTutors.SelectMany(tutorList => tutorList).Distinct());
        return formatedTutors;
    }
}