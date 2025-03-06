using OpenQA.Selenium.BiDi.Modules.Script;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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
                var title = _driver.FindElements(By.CssSelector(".title"));
                string? name = title.Select(x => x.Text.ToString())
                    .Where(x => x.Contains("Plan"))
                    .FirstOrDefault();
                if (name != null)
                {
                    int startingIndex = name.IndexOf("-");
                    int endIndex = name.IndexOf(",");
                    teacherName = name.Substring(startingIndex, endIndex - startingIndex);
                }

            return teacherName;
        }
           
        }
    }