using System;
using Core.Models;
using Infrastructure.Services.Abstract;
using OpenQA.Selenium;

namespace Infrastructure.Services
{
    public class DriverContainer : IDriverContainer
    {
        public List<WebDriverItem> WebDrivers { get; set; }

        public DriverContainer()
        {
            WebDrivers = new List<WebDriverItem>();
        }

        public IWebDriver GetDriver(string userId, string url)
        {
            // Use LINQ to find the first matching item in the WebDrivers list
            var driverItem = WebDrivers.FirstOrDefault(item => item.UserId == userId && item.Url == url);

            if (driverItem != null)
            {
                // Return the WebDriver associated with the matching item
                return driverItem.Driver;
            }
            else
            {
                if (WebDrivers.Count >= 50)
                {
                    return null;
                }

                WebDriverItem item = new WebDriverItem(userId, url);

                WebDrivers.Add(item);

                return item.Driver;
            }
        }
    }
}

