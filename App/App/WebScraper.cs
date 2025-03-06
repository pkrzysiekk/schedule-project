using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V131.Debugger;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App
{
    public class WebScraper
    {
        public ChromeDriver _driver;
        private CoursesHandler _coursesHandler;
        public WebScraper()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless=new");
            _driver = new ChromeDriver(chromeOptions);
            _coursesHandler = new CoursesHandler();
        }

        public List<string> GetTutorLinkList(string url)
        {
            _driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".coursediv")));
            var courseCard = _driver.FindElements(By.CssSelector(".coursediv"));
            List<string> list = new List<string>();
            List<Tutor> tutors=new List<Tutor>();
            foreach (var element in courseCard)
            {
                var a = element.FindElements(By.CssSelector("a"));
                var text = element.Text;
                
                Console.WriteLine(text);
                foreach (var link in a)
                {
                    string href;

                    href = link.GetAttribute("href");

                    if (href != null && href.Contains("type=10"))
                    {
                        list.Add(href);
                        var teacher = _coursesHandler.GetTeacherFullName(href);
                        if (teacher != null)
                        {
                            Console.WriteLine(teacher);
                        }

                    }

                }
            }
            return list;
        }

        

    }
}
