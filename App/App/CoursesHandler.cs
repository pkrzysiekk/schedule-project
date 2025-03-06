using OpenQA.Selenium.BiDi.Modules.Script;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace App
{
    public class CoursesHandler
    {
        private ChromeDriver _driver;
        public CoursesHandler()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless=new");
            _driver = new ChromeDriver(chromeOptions);
           
        }
        public string? GetTeacherFullName(string url)
        {
             string? teacherName= null;
            _driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".title")));

            var title = _driver.FindElements(By.CssSelector(".title"));
                string? name = title.Select(x => x.Text.ToString())
                    .Where(x => x.Contains("Plan"))
                    .FirstOrDefault();
                if (name != null)
                {
                    int startingIndex = name.IndexOf("-");
                    int endIndex = name.IndexOf(",");
                    teacherName = name.Substring(startingIndex+1, endIndex - startingIndex);
                }

            return teacherName;
        }
           
        }
    }