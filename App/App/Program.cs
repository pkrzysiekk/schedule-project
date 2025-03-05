using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

class Program
{
    static void Main(string[] args)
    {
        // instantiate a driver instance to control
        // Chrome in headless mode
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments("--headless=new"); // comment out for testing
        var driver = new ChromeDriver(chromeOptions);
        driver.Navigate().GoToUrl("https://plany.ubb.edu.pl/plan.php?type=2&id=12626&winW=1341&winH=946&loadBG=000000");
        var html = driver.PageSource;
        


        // scraping logic...

        // close the browser and release its resources
        driver.Quit();
    }
}
