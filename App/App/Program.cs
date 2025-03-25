using App.Controllers;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ScraperController scraperController = new ScraperController();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var formatedTutors = await scraperController.Scrape();
        sw.Stop();
        foreach (var tutor in formatedTutors)
        {
            Console.WriteLine($"Tutor: {tutor.Name}");
            Console.WriteLine($"Course: {tutor.Course.CourseShortName}");
            Console.WriteLine($"Full course name: {tutor.Course.CourseFullName}");
            Console.WriteLine($"Type: {tutor.Course.Type}");
            Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
            Console.WriteLine(tutor.Course.ScheduleType);
            Console.WriteLine("*********************");
        }
        Console.WriteLine($"elapsed: {sw.ElapsedMilliseconds / 1000}");
        Console.ReadLine();
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