using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V131.Debugger;
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
        public WebScraper()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless=new"); // comment out for testing
            _driver = new ChromeDriver(chromeOptions);
        }

        public List<string> GetTutorLinkList(string url)
        {
            _driver.Navigate().GoToUrl(url);
            var productElements = _driver.FindElements(By.CssSelector(".coursediv"));
            List<string> list = new List<string>();

            foreach (var element in productElements)
            {
                var a = element.FindElements(By.CssSelector("a"));
                foreach (var link in a)
                {
                    var href = link.GetAttribute("href");
                    if (href != null && href.Contains("type=10"))
                    {
                        list.Add(href);  // Add matching links to the list
                    }
                }
            }

            return list;
        }

        public List<Course> GetCourses(List<string> urls) 
        {
            List<Course> courses = new List<Course>();

            foreach (var url in urls)
            {
                _driver.Navigate().GoToUrl(url);
                var title = _driver.FindElements(By.CssSelector(".title"));
                string name=title.Select(x => x.Text.ToString())
                    .Where(x=>x.Contains("Plan"))
                    .FirstOrDefault();
                Console.WriteLine(name);



            }
           return courses;
        }

    }
}
