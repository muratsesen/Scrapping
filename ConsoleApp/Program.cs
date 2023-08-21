// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.Json;

// var madridResult = Madrid("1254874","","");
// System.Console.WriteLine("Madrid result: " + madridResult);


void ChromeSession()
{
    IWebDriver driver = new ChromeDriver();

    driver.Navigate().GoToUrl("https://www3.wipo.int/madrid/monitor/en/");

    var title = driver.Title;
    // Assert.AreEqual("Web form", title);

    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

    var textBox = driver.FindElement(By.Name("my-text"));
    var submitButton = driver.FindElement(By.TagName("button"));

    textBox.SendKeys("Selenium");
    submitButton.Click();

    var message = driver.FindElement(By.Id("message"));
    var value = message.Text;
    // Assert.AreEqual("Received!", value);

    driver.Quit();
}
