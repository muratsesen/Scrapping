using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.Json;
using Core.Models;

namespace Infrastructure.Services;
public class MadridService : IMadridService
{
    private readonly IWebDriver driver;
    public MadridService(IWebDriver _driver)
    {
        driver = _driver;
    }

    public string GetList(MadridSearchModel model)
    {
        System.Console.WriteLine("Searching in Madird");
        var madridResult = ScrapeList(model);
        System.Console.WriteLine("Madrid result: " + madridResult);
        return madridResult;
    }
    public string GetDetail(MadridSearchModel model)
    {
        System.Console.WriteLine("Searching in Madird");
        var madridResult = ScrapeDetail(model);
        System.Console.WriteLine("Madrid result: " + madridResult);
        return madridResult;
    }
    public static string ScrapeDetail(MadridSearchModel model)
    {
        return "";
    }

    public string ScrapeList(MadridSearchModel model)
    {
        driver.Navigate().GoToUrl("https://www3.wipo.int/madrid/monitor/en/");

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

        var advanceSearchButton = driver.FindElement(By.Id("advancedModeLink"));
        advanceSearchButton.Click();

        var intRegistrationInput = driver.FindElement(By.Id("IRN_input"));
        var basicNoInput = driver.FindElement(By.Id("BN_input"));
        var holderInput = driver.FindElement(By.Id("HOL_input"));

        if (!string.IsNullOrEmpty(model.IntRegNo))
        {
            intRegistrationInput.SendKeys(model.IntRegNo);
            intRegistrationInput.SendKeys(Keys.Enter);

            return MultipleRow(driver);
        }
        else if (!string.IsNullOrEmpty(model.BaseNo))
        {
            basicNoInput.SendKeys(model.BaseNo);
            basicNoInput.SendKeys(Keys.Enter);

            return MultipleRow(driver);
        }
        else if (!string.IsNullOrEmpty(model.HolderName))
        {
            holderInput.SendKeys(model.HolderName);
            holderInput.SendKeys(Keys.Enter);

            return MultipleRow(driver);
        }

        return "";

    }

    public string MultipleRow(IWebDriver driver)
    {
        IWebElement pageCountElement = driver.FindElement(By.ClassName("pageCount"));

        string pageCountText = pageCountElement.Text;

        if (string.IsNullOrEmpty(pageCountText))
            return "";
        var pageCount = ExtractNumber(pageCountText);
        if (pageCount == -1) return "";


        List<SearchResultItem> searchResultItems = new List<SearchResultItem>();
        for (int i = 1; i <= pageCount; i++)
        {
            //TODO out
            var pageData = ProcessPage(driver);

            searchResultItems.AddRange(pageData);

            try
            {
                IWebElement nextPageButton = driver.FindElement(By.CssSelector("a.hasTip.ui-button[aria-label='next page']"));

                nextPageButton.Click();
            }
            catch { continue; }
        }
        return JsonSerializer.Serialize(searchResultItems);
    }
    public List<SearchResultItem> ProcessPage(IWebDriver driver)
    {
        List<SearchResultItem> searchResultItems = new List<SearchResultItem>();

        IWebElement table = driver.FindElement(By.Id("gridForsearch_pane"));
        IWebElement tableBody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tableBody.FindElements(By.TagName("tr"));


        for (int i = 1; i < rows.Count; i++)
        {
            IWebElement row = rows[i];

            IWebElement brandElement = null;
            string srcAttributeValue = "";
            IWebElement statusDiv = null;
            IWebElement originElement = null;
            IWebElement holderElement = null;
            IWebElement irnElement = null;
            IWebElement rdElement = null;
            IWebElement ncElement = null;
            IWebElement vcsElement = null;

            try { brandElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_BRAND']")); } catch (NoSuchElementException) { }

            try
            {
                IWebElement imgTdElement = driver.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IMG']"));
                IWebElement imgElement = imgTdElement.FindElement(By.TagName("img"));
                srcAttributeValue = imgElement.GetAttribute("src");
            }
            catch (NoSuchElementException ex)
            {
                System.Console.WriteLine("Eleman bulunamadı");
            }

            try
            {
                IWebElement statusElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_STATUS']"));
                statusDiv = statusElement.FindElement(By.XPath(".//div[1]"));
            }
            catch (NoSuchElementException) { }

            try { originElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_OO']")); } catch (NoSuchElementException) { }
            try { holderElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_HOL']")); } catch (NoSuchElementException) { }
            try { irnElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IRN']")); } catch (NoSuchElementException) { }
            try { rdElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_RD']")); } catch (NoSuchElementException) { }
            try { ncElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_NC']")); } catch (NoSuchElementException) { }
            try { vcsElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_VCS']")); } catch (NoSuchElementException) { }

            // Extract the text content from the elements
            string brand = brandElement != null ? brandElement.Text : "";
            string imageUrl = srcAttributeValue;
            string status = statusDiv != null ? statusDiv.Text : "";
            string origin = originElement != null ? originElement.Text : "";
            string holder = holderElement != null ? holderElement.Text : "";
            string irn = irnElement != null ? irnElement.Text : "";
            string rd = rdElement != null ? rdElement.Text : "";
            string nc = ncElement != null ? ncElement.Text : "";
            string vcs = vcsElement != null ? vcsElement.Text : "";

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

            searchResultItems.Add(searchResultItem);
        }
        return searchResultItems;
    }

    static int ExtractNumber(string input)
    {
        int startIndex = input.IndexOf('/') + 1;
        int endIndex = input.Length;

        if (startIndex >= 0 && startIndex < endIndex)
        {
            string numberPart = input.Substring(startIndex, endIndex - startIndex);
            if (int.TryParse(numberPart, out int result))
            {
                return result;
            }
        }

        return -1; // Return a default value if extraction fails
    }

    // SearchResultItem ProcessRow(IWebDriver driver, int pageCount, List<SearchResultItem> searchResultItems)
    // {
    //      if (pageCount == 1)
    //     {

    //         return ExtractDataFromRow(driver, searchResultItems);
    //     }
    //     IWebElement table = driver.FindElement(By.Id("gridForsearch_pane"));
    //     IWebElement tableBody = table.FindElement(By.TagName("tbody"));

    //     IWebElement brandElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_BRAND']"));

    //     IWebElement imgTdElement = driver.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IMG']"));
    //     IWebElement imgElement = imgTdElement.FindElement(By.TagName("img"));
    //     string srcAttributeValue = imgElement.GetAttribute("src");

    //     IWebElement statusElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_STATUS']"));
    //     IWebElement statusDiv = statusElement.FindElement(By.XPath(".//div[1]"));

    //     IWebElement originElement = tableBody.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_OO']"));

    // }
}