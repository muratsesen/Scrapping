using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Core.Models
{
    public class WebDriverItem
    {
        public string UserId { get; set; }
        public string Url { get; set; }
        public IWebDriver Driver { get; set; }

        public WebDriverItem(string userId, string url)
        {
            UserId = userId;
            Url = url;
            Driver = new FirefoxDriver();
            Driver.Navigate().GoToUrl(Url);
        }
    }
}

