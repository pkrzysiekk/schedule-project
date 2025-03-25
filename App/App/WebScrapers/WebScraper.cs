using App.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V131.Network;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace App.WebScrapers;

public class WebScraper : IDisposable
{
    private ChromeDriver _driver;
    private CoursesHandler _coursesHandler;
    private bool _disposed = false;
    private static ConcurrentDictionary<string, string> _namesDictionary = new();

    static WebScraper()
    {
        string json;
        try
        {
            json = File.ReadAllText("dictionary.json");
        }
        catch
        {
            return;
        }
        _namesDictionary = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json);
    }

    public WebScraper()
    {
        var chromeOptions = new ChromeOptions();

        chromeOptions.AddArguments("--headless");
        chromeOptions.AddArguments("--no-sandbox");
        chromeOptions.AddArguments("--disable-dev-shm-usage");
        chromeOptions.AddArguments("--disable-software-rasterizer");
        chromeOptions.AddArguments("--disable-gpu");
        chromeOptions.AddArguments("--disk-cache-size=0");

        _driver = new ChromeDriver(chromeOptions);
        _coursesHandler = new CoursesHandler();
    }

    public Course? GetCourse(string text)
    {
        Course course = new Course();
        int pivot = text.IndexOf(",");
        int endIndex = text.IndexOf('\r');
        if (pivot == -1 || endIndex == -1)
        {
            return null;
        }
        course.CourseShortName = text.Substring(0, pivot);
        course.Type = text.Substring(pivot + 1, endIndex - pivot);
        return course;
    }

    public Dictionary<string, string> GetSubjectsFullName(string[] strings)
    {
        string[] filtered = strings.Where(x => x.Contains("występowanie:")).ToArray();
        string pattern = @"^([\w()]+)\s*(?:\([^)]+\))?\s*-\s*([^,(]+)";

        Dictionary<string, string> courses = new();
        foreach (var item in filtered)
        {
            Match match = Regex.Match(item, pattern);
            if (match.Success)
            {
                string shortName = match.Groups[1].Value;
                string fullName = match.Groups[2].Value;
                if (fullName.Contains("zpp - Projekt - zespołowe przedsięwzięcie programistyczne"))
                {
                    courses.Add("P-zpp", "Projekt - zespołowe przedsięwzięcie programistyczne");
                    continue;
                }
                try
                {
                    courses.Add(shortName, fullName);
                }
                catch
                {
                    continue;
                }
            }
        }
        return courses;
    }

    public static void SaveDictionary()
    {
        var json = JsonConvert.SerializeObject(_namesDictionary);
        File.WriteAllText("dictionary.json", json);
    }

    public List<Tutor> StartScraping(string url, string scheduleType)
    {
        List<Tutor> tutors = [];
        List<Tutor> list1 = [];
        List<Tutor> list2 = [];
        switch (scheduleType)
        {
            case "stacjonarne":
                list1 = ScrapeSelectedSchedule(url, 2, scheduleType);
                list1 = ScrapeSelectedSchedule(url, 3, scheduleType);
                tutors.AddRange(list1);
                tutors.AddRange(list2);
                break;

            case "niestacjonarne":
                list1 = ScrapeSelectedSchedule(url, 2, scheduleType);
                tutors.AddRange(list1);
                break;

            case "stacjonarne magisterskie":
                list1 = ScrapeSelectedSchedule(url, 2, scheduleType);
                list2 = ScrapeSelectedSchedule(url, 3, scheduleType);
                tutors.AddRange(list1);
                tutors.AddRange(list2);
                break;
        }

        return tutors;
    }

    public List<Tutor> ScrapeSelectedSchedule(string url, int menuOption, string schedeuleType)
    {
        int timeoutInterval = 5;
        _driver.Navigate().GoToUrl(url);
        WebDriverWait waitForSelection = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInterval));
        try
        {
            waitForSelection.Until(ExpectedConditions.ElementExists(By.Id("wBWeek")));
        }
        catch (WebDriverTimeoutException)
        {
            return [];
        }
        IWebElement selectElement = _driver.FindElement(By.Id("wBWeek"));
        SelectElement select = new SelectElement(selectElement);
        select.SelectByIndex(menuOption);
        IWebElement searchButton = _driver.FindElement(By.Id("wBButton"));
        searchButton.Click();

        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInterval));
        try
        {
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".coursediv")));
        }
        catch (WebDriverTimeoutException)
        {
            return [];
        }
        var courseCard = _driver.FindElements(By.CssSelector(".coursediv"));
        var data = _driver.FindElements(By.CssSelector(".data"));
        var subjects = data.Where(x => x.Text.Contains("najechanie")).FirstOrDefault();
        Dictionary<string, string> courses = new();

        if (subjects is not null)
        {
            var text = subjects.Text;
            string[] strings = text.Split("\n");
            courses = GetSubjectsFullName(strings);
        }

        List<Tutor> tutors = new List<Tutor>();

        foreach (var element in courseCard)
        {
            var a = element.FindElements(By.CssSelector("a"));
            var text = element.Text;
            Course? course = GetCourse(text);

            Tutor tutor = new Tutor();
            if (course == null)
            {
                continue;
            }
            course.ScheduleType = schedeuleType;
            foreach (var link in a)
            {
                string href;

                href = link.GetAttribute("href");
                var hrefText = link.Text;
                if (href != null && href.Contains("type=10"))
                {
                    _namesDictionary.TryGetValue(hrefText, out string? teacher);
                    if (teacher == null)
                    {
                        teacher = _coursesHandler.GetTeacherFullName(href);
                        if (teacher == null)
                        {
                            continue;
                        }
                        _namesDictionary.TryAdd(hrefText, teacher);
                    }
                    course.CourseFullName = courses[course.CourseShortName];

                    tutor.Name = teacher;
                    tutor.Course = course;
                    if (course.Type.Contains("wyk"))
                    {
                        tutor.IsLead = true;
                    }
                    tutors.Add(tutor);
                }
            }
        }
        return tutors;
    }

    public void Dispose()
    {
        Quit();
    }

    public void Quit()
    {
        if (!_disposed)
        {
            _coursesHandler.Quit();
            _driver.Quit();
            _disposed = true;
        }
    }

    ~WebScraper()
    {
        Quit();
    }
}