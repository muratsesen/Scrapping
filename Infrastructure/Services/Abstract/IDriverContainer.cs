using System;
using OpenQA.Selenium;

namespace Infrastructure.Services.Abstract
{
    public interface IDriverContainer
    {
        IWebDriver GetDriver(string userId, string url);
    }
}

