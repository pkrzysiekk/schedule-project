using App.Models;
using App.WebScrapers;
using Newtonsoft.Json;

namespace App.Controllers;

public class ScraperController
{
    private string _json;
    private List<string> _links;

    public ScraperController()
    {
        _json = File.ReadAllText("links.json");
        _links = JsonConvert.DeserializeObject<List<string>>(_json);
    }

    public async Task<List<Tutor>> Scrape()
    {
        List<Task<List<Tutor>>> tasks = new();

        foreach (var link in _links)
        {
            tasks.Add(Task.Run(() =>
            {
                using WebScraper scraper = new WebScraper();
                var result = scraper.GetTutorLinkList(link);
                scraper.Quit();
                return result;
            }));
        }

        List<Tutor>[] allTutors = await Task.WhenAll(tasks);

        List<Tutor> formatedTutors = new List<Tutor>();
        formatedTutors.AddRange(allTutors.SelectMany(tutorList => tutorList));
        return formatedTutors;
    }
}