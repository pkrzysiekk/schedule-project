using App.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V131.Network;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Concurrent;

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
        course.courseShortName = text.Substring(0, pivot);
        course.type = text.Substring(pivot + 1, endIndex - pivot);
        return course;
    }

    public Dictionary<string, string> GetSubjectsFullName(string[] strings)
    {
        string[] filtered = strings.Where(x => x.Contains("występowanie:")).ToArray();
        Dictionary<string, string> courses = new();
        foreach (var item in filtered)
        {
            int pivot = item.IndexOf("-");
            int endIndex = item.IndexOf(",") - 1;
            string shortName = item.Substring(0, pivot - 1);
            string fullName = item.Substring(pivot + 1, endIndex - pivot);
            shortName = shortName.Trim();
            fullName = fullName.Trim();
            courses.Add(shortName, fullName);
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
        List<Tutor> tutors = new List<Tutor>();

        if (scheduleType == "stacjonarne")
        {
            var list1 = ScrapeSelectedSchedule(url, 2);
            var list2 = ScrapeSelectedSchedule(url, 3);
            tutors.AddRange(list1);
            tutors.AddRange(list2);
        }
        else
        {
            var list1 = ScrapeSelectedSchedule(url, 2, false);
            tutors.AddRange(list1);
        }
        return tutors;
    }

    public List<Tutor> ScrapeSelectedSchedule(string url, int menuOption, bool isFullTime = true)
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
            course.isFullTime = isFullTime ? true : false;
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
                        _namesDictionary.TryAdd(hrefText, teacher);
                    }
                    try
                    {
                        course.courseFullName = courses[course.courseShortName];
                    }
                    catch
                    {
                        course.courseFullName = "Projekt - zespołowe przedsięwzięcie programistyczne";
                    }
                    tutor.Name = teacher;
                    tutor.Course = course;
                    if (course.type.Contains("wyk"))
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