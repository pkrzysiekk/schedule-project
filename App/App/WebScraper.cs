using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V131.Debugger;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App
{
    public class WebScraper :IDisposable
    {
        private ChromeDriver _driver;
        private CoursesHandler _coursesHandler;
        private bool _disposed = false;
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
            if(pivot==-1 || endIndex == -1)
            {
                return null;
            }
            course.courseShortName = text.Substring(0, pivot);
            course.type = text.Substring(pivot+1,endIndex-pivot);
            return course;
        }
        public Dictionary<string,string> GetSubjectsFullName(string[] strings)
        {
            string[] filtered = strings.Where(x => x.Contains("występowanie:")).ToArray();
            Dictionary<string, string> courses = new();
            foreach (var item in filtered)
            {
                int pivot = item.IndexOf("-");
                int endIndex = item.IndexOf(",")-1;
                string shortName = item.Substring(0, pivot-1);
                string fullName = item.Substring(pivot + 1,endIndex-pivot);
                shortName= shortName.Trim();
                fullName = fullName.Trim();
                courses.Add(shortName, fullName);
            }
            return courses;
        }
        public List<Tutor> GetTutorLinkList(string url)
        {
            _driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".coursediv")));

            }
            catch (WebDriverTimeoutException)
            {
                return [];
            }
            var courseCard = _driver.FindElements(By.CssSelector(".coursediv"));
            var data=_driver.FindElements(By.CssSelector(".data"));
            var subjects= data.Where(x => x.Text.Contains("najechanie")).FirstOrDefault();
            Dictionary<string, string> courses = new();

            if (subjects is not null)
            {
                var text = subjects.Text;
                string[] strings = text.Split("\n");
                courses = GetSubjectsFullName(strings);
            }

            List<Tutor> tutors=new List<Tutor>();

            foreach (var element in courseCard)
            {
                var a = element.FindElements(By.CssSelector("a"));
                var text = element.Text;
                Course? course= GetCourse(text);
                Tutor tutor = new Tutor();
                if (course == null)
                {
                    continue;
                }
                foreach (var link in a)
                {
                    string href;

                    href = link.GetAttribute("href");

                    if (href != null && href.Contains("type=10"))
                    {
                        var teacher = _coursesHandler.GetTeacherFullName(href);
                        if (teacher != null)
                        {
                            course.courseFullName = courses[course.courseShortName];
                            tutor.Name = teacher;
                            tutor.Course = course;
                            if (course.type.Contains("wyk"))
                            {
                                tutor.IsLead = true;
                            }
                            if(tutors.Contains(tutor))
                            {
                                continue;
                            }
                                tutors.Add(tutor);
                        }

                    }

                }
            }
            return tutors;
        }
        public void Dispose()
        {
            this.Quit();
        }
        public void Quit()
        {
            if(!_disposed)
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
}
