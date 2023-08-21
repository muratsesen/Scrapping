using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.Json;

namespace ConsoleApp.Services;
public static class MadridService
{
    public static string GetMadrid(string regNo,string? basicNo,string? holder)
    {
        var madridResult = Madrid(regNo, basicNo, holder);
        System.Console.WriteLine("Madrid result: " + madridResult);
        return madridResult;
    }
    static string Madrid(string? IRNo, string basicNo, string _holder)
    {
        IWebDriver driver = new FirefoxDriver();

        driver.Navigate().GoToUrl("https://www3.wipo.int/madrid/monitor/en/");

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

        var advanceSearchButton = driver.FindElement(By.Id("advancedModeLink"));
        advanceSearchButton.Click();

        var intRegistrationInput = driver.FindElement(By.Id("IRN_input"));
        var basicNoInput = driver.FindElement(By.Id("BN_input"));
        var holderInput = driver.FindElement(By.Id("HOL_input"));

        if (!string.IsNullOrEmpty(IRNo))
        {
            intRegistrationInput.SendKeys(IRNo);
            intRegistrationInput.SendKeys(Keys.Enter);

            return SingleRow(driver);
        }
        else if (!string.IsNullOrEmpty(basicNo))
        {
            basicNoInput.SendKeys(basicNo);
            basicNoInput.SendKeys(Keys.Enter);

            return MultipleRow(driver);
        }
        else if (!string.IsNullOrEmpty(_holder))
        {
            holderInput.SendKeys(_holder);
            holderInput.SendKeys(Keys.Enter);

            return MultipleRow(driver);
        }

        return "";

    }
    static string SingleRow(IWebDriver driver)
    {
        IWebElement table = driver.FindElement(By.Id("gridForsearch_pane"));
        IWebElement tableBody = table.FindElement(By.TagName("tbody"));

        IWebElement brandElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_BRAND']"));

        IWebElement imgTdElement = driver.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IMG']"));
        IWebElement imgElement = imgTdElement.FindElement(By.TagName("img"));
        string srcAttributeValue = imgElement.GetAttribute("src");

        IWebElement statusElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_STATUS']"));
        IWebElement statusDiv = statusElement.FindElement(By.XPath(".//div[1]"));

        IWebElement originElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_OO']"));
        IWebElement holderElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_HOL']"));
        IWebElement irnElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IRN']"));
        IWebElement rdElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_RD']"));
        IWebElement ncElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_NC']"));
        IWebElement vcsElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_VCS']"));

        // Extract the text content from the elements
        string brand = brandElement.Text;
        string imageUrl = srcAttributeValue;
        string status = statusDiv.Text;
        string origin = originElement.Text;
        string holder = holderElement.Text;
        string irn = irnElement.Text;
        string rd = rdElement.Text;
        string nc = ncElement.Text;
        string vcs = vcsElement.Text;

        // Print the extracted data
        Console.WriteLine($"brand: {brand}");
        Console.WriteLine($"image: {imageUrl}");
        Console.WriteLine($"status: {status}");
        Console.WriteLine($"origin: {origin}");
        Console.WriteLine($"holder: {holder}");
        Console.WriteLine($"irn: {irn}");
        Console.WriteLine($"rd: {rd}");
        Console.WriteLine($"nc: {nc}");
        Console.WriteLine($"vcs: {vcs}");

        SearchResultItem searchResultItem = new SearchResultItem()
        {
            Brand = brand,
            ImageUrl = imageUrl,
            Status = status,
            Origin = origin,
            Holder = holder,
            RegNo = irn,
            RegDate = rd,
            NiceCI = nc,
            ViennaCI = vcs,
        };

        driver.Quit();

        return JsonSerializer.Serialize(searchResultItem);
    }
    static string MultipleRow(IWebDriver driver)
    {
        IWebElement pageCountElement = driver.FindElement(By.Id("pageCount"));

        string pageCountText = pageCountElement.Text;

        if (!string.IsNullOrEmpty(pageCountText))
        {
        }
        return "";
    }

}