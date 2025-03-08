using App.Models;
using App.WebScrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class ScraperController
    {
        public async Task<List<Tutor>> Scrape(List<string> strings)
        {
            List<Task<List<Tutor>>> tasks = new();

            foreach (var link in strings)
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
}
