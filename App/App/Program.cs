using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using App;
using Newtonsoft.Json;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {

        Stopwatch sw = new Stopwatch();

        string json =File.ReadAllText("links.json");
        List<string> links = JsonConvert.DeserializeObject<List<string>>(json);
        List<Task<List<Tutor>>> tasks = new();
        sw.Start();

        foreach (var link in links)
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
        sw.Stop();


        List<Tutor> formatedTutors = new List<Tutor>();
        formatedTutors.AddRange(allTutors.SelectMany(tutorList => tutorList));

        foreach (var tutor in formatedTutors)
        {
            Console.WriteLine($"Tutor: {tutor.Name}");
            Console.WriteLine($"Course: {tutor.Course.courseShortName}");
            Console.WriteLine($"Full course name: {tutor.Course.courseFullName}");
            Console.WriteLine($"Type: {tutor.Course.type}");
            Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
            Console.WriteLine("*********************");

        }
        Console.WriteLine("Enter subject name: ");
        Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds / 1000} s");
        Console.WriteLine("Enter subject name: ");

        string name = Console.ReadLine();
        var tutorToFind = formatedTutors.Find(x => x.Course.courseShortName == name && x.IsLead);

        if (tutorToFind != null)
        {
            Console.WriteLine($"Tutor found: {tutorToFind.Name}");
        }
        else
        {
            Console.WriteLine("Tutor not found");
        }
        


    }
}
