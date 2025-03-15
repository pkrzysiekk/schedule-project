using App.Controllers;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ScraperController scraperController = new ScraperController();

        var formatedTutors = await scraperController.Scrape();

        foreach (var tutor in formatedTutors)
        {
            Console.WriteLine($"Tutor: {tutor.Name}");
            Console.WriteLine($"Course: {tutor.Course.courseShortName}");
            Console.WriteLine($"Full course name: {tutor.Course.courseFullName}");
            Console.WriteLine($"Type: {tutor.Course.type}");
            Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
            Console.WriteLine("*********************");
        }
        //Console.WriteLine("Enter subject name: ");

        //string name = Console.ReadLine();
        //var tutorToFind = formatedTutors.Find(x => x.Course.courseShortName == name && x.IsLead);

        //if (tutorToFind != null)
        //{
        //    Console.WriteLine($"Tutor found: {tutorToFind.Name}");
        //}
        //else
        //{
        //    Console.WriteLine("Tutor not found");
        //}
    }
}