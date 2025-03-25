using App.Models;
using App.WebScrapers;
using Newtonsoft.Json;

namespace App.Controllers;

public class ScraperController
{
    private string _json;
    private List<ScheduleLink>? _links;

    public ScraperController()
    {
        try
        {
            _json = File.ReadAllText("links.json");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File links.json not found");
            Environment.Exit(1);
        }

        _links = JsonConvert.DeserializeObject<List<ScheduleLink>>(_json);
    }

    public async Task<List<Tutor>> Scrape()
    {
        List<Task<List<Tutor>>> tasks = new();

        foreach (var link in _links)
        {
            tasks.Add(Task.Run(() =>
            {
                List<Tutor> result = new();
                using WebScraper scraper = new WebScraper();

                try
                {
                    result = scraper.StartScraping(link.Url, link.Type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error whiile scrapping {link.Url}: {ex.Message}");
                }
                finally
                {
                    scraper.Quit();
                }

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