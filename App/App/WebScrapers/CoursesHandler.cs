using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace App.WebScrapers;

public class CoursesHandler : IDisposable
{
    private ChromeDriver _driver;
    private bool _disposed = false;

    public CoursesHandler()
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments("--headless=new");
        chromeOptions.AddArgument("--remote-debugging-port=0");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArguments("--log-level=3");
        chromeOptions.AddArguments("--disk-cache-size=0");
        chromeOptions.AddArguments("--disable-software-rasterizer");
        chromeOptions.AddArguments("--disable-gpu");

        _driver = new ChromeDriver(chromeOptions);
    }

    public string? GetTeacherFullName(string url)
    {
        string? teacherName = null;
        _driver.Navigate().GoToUrl(url);
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
        try
        {
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".title")));
        }
        catch (WebDriverTimeoutException)
        {
            return null;
        }

        var title = _driver.FindElements(By.CssSelector(".title"));
        string? name = title.Select(x => x.Text.ToString())
            .Where(x => x.Contains("Plan"))
            .FirstOrDefault();
        if (name != null)
        {
            int startingIndex = name.IndexOf("-") + 1;
            int endIndex = name.IndexOf(",");
            teacherName = name.Substring(startingIndex, endIndex - startingIndex);
        }

        return teacherName;
    }

    public void Dispose()
    {
        Quit();
    }

    public void Quit()
    {
        if (!_disposed)
        {
            _driver.Quit();
            _disposed = true;
        }
    }

    ~CoursesHandler()
    {
        Quit();
    }
}