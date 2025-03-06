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
                using WebScraper scraper = new WebScraper();  // Tworzymy scraper w Task.Run()
                var result = scraper.GetTutorLinkList(link);
                scraper.Quit();  // Zamykamy scraper po zakończeniu zadania
                return result;
            }));
        }
        List<Tutor>[] allTutors = await Task.WhenAll(tasks);
        sw.Stop();


        List<Tutor> formatedTutors = new List<Tutor>();
        formatedTutors.AddRange(allTutors.SelectMany(tutorList => tutorList));

        foreach (var tutor in formatedTutors)
        {
            Console.WriteLine("*********************");
            Console.WriteLine($"Tutor: {tutor.Name}");
            Console.WriteLine($"Course: {tutor.Course.courseName}");
            Console.WriteLine($"Type: {tutor.Course.type}");
            Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
            Console.WriteLine("*********************");

        }
        Console.WriteLine("Enter subject name: ");
        Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds / 1000} s");
        Console.WriteLine("Enter subject name: ");

        string name = Console.ReadLine();
        var tutorToFind = formatedTutors.Find(x => x.Course.courseName == name && x.IsLead);

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
