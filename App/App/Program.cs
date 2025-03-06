using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using App;

class Program
{
    static void Main(string[] args)
    {
        // instantiate a driver instance to control
        // Chrome in headless mode
        WebScraper scraper = new WebScraper();

        var tutors = scraper.GetTutorLinkList("https://plany.ubb.edu.pl/plan.php?type=2&id=12626&winW=1341&winH=946&loadBG=000000");
        foreach (var tutor in tutors)
        {
            Console.WriteLine("*********************");
            Console.WriteLine($"Tutor: {tutor.Name}");
            Console.WriteLine($"Course: {tutor.Course.courseName}");
            Console.WriteLine($"Type: {tutor.Course.type}");
            Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
            Console.WriteLine("*********************");

        }
        Console.WriteLine("Enter subject name: ");
        string name=Console.ReadLine();
        var tutorToFind = tutors.Find(x => x.Course.courseName==name && x.IsLead);
        if (tutorToFind != null)
        {
            Console.WriteLine($"Tutor found: {tutorToFind.Name}");
        }
        else
        {
            Console.WriteLine("Tutor not found");
        }
        scraper._driver.Quit();

      
    }
}
