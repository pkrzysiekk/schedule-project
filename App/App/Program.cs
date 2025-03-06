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

        var list = scraper.GetTutorLinkList("https://plany.ubb.edu.pl/plan.php?type=2&id=12626&winW=1341&winH=946&loadBG=000000");

        scraper._driver.Quit();

      
    }
}
