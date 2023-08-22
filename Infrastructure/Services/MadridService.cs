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

        var result = ScrapeList(model);

        return result;
    }
    public string GetDetail(MadridSearchModel model)
    {
        var madridResult = ScrapeDetail(model);
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

        if (!string.IsNullOrEmpty(model.RegistrationNo))
        {
            intRegistrationInput.SendKeys(model.RegistrationNo);
        }
        else if (!string.IsNullOrEmpty(model.BaseNo))
        {
            basicNoInput.SendKeys(model.BaseNo);
        }
        else if (!string.IsNullOrEmpty(model.HolderName))
        {
            holderInput.SendKeys(model.HolderName);
        }

        var searchButton = driver.FindElement(By.CssSelector("a.noPrint.searchButton.bigButton.ui-button.ui-widget.ui-state-default.ui-corner-all.ui-button-text-icon-secondary"));
        searchButton.Click();

        return ProcessResult();
    }

    public string ProcessResult()
    {
        IWebElement pageCountElement = driver.FindElement(By.ClassName("pageCount"));

        string pageCountText = pageCountElement.Text;

        if (string.IsNullOrEmpty(pageCountText))
            return "";

        var pageCount = ExtractNumber(pageCountText);

        if (pageCount == -1) return "";

        if (pageCount == 1)
        {
            var rowCount = RowCount();//tek satır olsa da iki geliyor çünkü görünmeyen bir satır var
            if (rowCount == 0) return "";

            if (rowCount == 2)
            {
                //Tek satır veri var. Detayını dönebilir
                SearchResultDetail detail = ProcessPageWithDetail();

                SearchResultDetail[] searchResultDetails = new SearchResultDetail[] { detail };
                var serializedResult = JsonSerializer.Serialize(searchResultDetails);
                return serializedResult;
            }
        }

        //Buraya geldiyse birden çok satır var. Çok elemanlı liste dönecek.
        List<SearchResultDetail> searchResultItems = new List<SearchResultDetail>();
        for (int i = 1; i <= pageCount; i++)
        {
            //TODO out
            var pageData = ProcessPage();

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
    public int RowCount()
    {
        IWebElement table = driver.FindElement(By.Id("gridForsearch_pane"));
        IWebElement tableBody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tableBody.FindElements(By.TagName("tr"));

        return rows.Count;

    }
    public SearchResultDetail ProcessPageWithDetail()
    {
        IWebElement table = driver.FindElement(By.Id("gridForsearch_pane"));
        IWebElement tableBody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tableBody.FindElements(By.TagName("tr"));
        IWebElement row = rows[1];//İki satır var. Birincisi boş. İkinci satırı işleyeceğiz

        IWebElement brandElement = null;
        string srcAttributeValue = "";
        IWebElement statusDiv = null;
        IWebElement originElement = null;
        IWebElement holderElement = null;
        IWebElement irnElement = null;

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
        try { holderElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_HOL']")); } catch (NoSuchElementException) { }
        try { irnElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IRN']")); } catch (NoSuchElementException) { }

        string brand = brandElement != null ? brandElement.Text : "";
        string imageUrl = srcAttributeValue;
        string status = statusDiv != null ? statusDiv.Text : "";//TODO null geliyor
        string holder = holderElement != null ? holderElement.Text : "";//TODO
        string irn = irnElement != null ? irnElement.Text : "";

        SearchResultDetail searchResultItem = new SearchResultDetail()
        {
            Brand = brand,
            ImageUrl = imageUrl,
            Status = status,
            // Holder = holder,
            RegistrationNo = irn
        };

        //Detail part
        if (brandElement != null)
        {
            brandElement.Click();
        }
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1000);
        //Select table by class name markInformation
        IWebElement table2 = driver.FindElement(By.CssSelector("table.markInformation"));

        // Tablodaki satırı seç
        IWebElement row2 = table2.FindElement(By.CssSelector("tbody tr"));

        // Satırdaki td elemanlarını seç
        IWebElement trademarkImage = row2.FindElement(By.CssSelector("td.mark div img"));
        IWebElement holderCell = row2.FindElement(By.CssSelector("td.name div div div.lapin.client"));
        IWebElement registrationDateCell = row2.FindElement(By.CssSelector("td.date div"));
        IWebElement expirationDateCell = row2.FindElements(By.CssSelector("td.date div"))[1];
        //IWebElement niceCell = row2.FindElement(By.CssSelector("td.nice div"));

        // Verileri al
        string trademark = trademarkImage.GetAttribute("src");
        string holder2 = holderCell.Text;
        string registrationDate = registrationDateCell.Text;
        string expirationDate = expirationDateCell.Text;
        //string nice = niceCell.Text;//TODO gereksizse sil

        searchResultItem.RegistrationDate = registrationDate;
        searchResultItem.RegistrationEndDate = expirationDate;

        //-----
        IWebElement currentStatusDataDiv = driver.FindElement(By.CssSelector("div.fragment-content.box_content.retreci.currentStatusData"));
        var currentStatusDataDivText = currentStatusDataDiv.Text;//TODO ne alaka

        IWebElement descriptionDiv = currentStatusDataDiv.FindElement(By.CssSelector("div.description"));

        // p class'ına sahip div'leri seç
        var pDivs = descriptionDiv.FindElements(By.CssSelector("div.p"));

        //151,540,180 yukarda alınıyor.
        var requestedCodes = new string[] { "732", "740", "511", "832" };
        var filteredDivs = pDivs.Where(pDiv =>
        {
            var inidCode = "";
            try
            {
                inidCode = pDiv.FindElement(By.CssSelector("div.inid div.inidCode")).Text;
            }
            catch (NoSuchElementException ex)
            {
                System.Console.WriteLine("Eleman bulunamadı");
            }
            return requestedCodes.Contains(inidCode);
        });

        foreach (var pDiv in filteredDivs)
        {
            var inidCode = pDiv.FindElement(By.CssSelector("div.inid div.inidCode")).Text;
            Console.WriteLine("inidCode: " + inidCode);

            //if (inidCode == "180")
            //{
            //    var textDiv = pDiv.FindElement(By.CssSelector("div.text"));
            //    var expectedEndDate = textDiv.FindElement(By.CssSelector("div:first-child")).Text;
            //}
            if (inidCode == "732")
            {

                var textDiv = pDiv.FindElement(By.CssSelector("div.text"));
                var applicantTitle = textDiv.FindElement(By.CssSelector("div:nth-child(1)")).Text;
                var addres1 = textDiv.FindElement(By.CssSelector("div:nth-child(2)")).Text;
                var addres2Div = textDiv.FindElement(By.CssSelector("div:nth-child(3)"));
                var addres2Text = addres2Div.Text;
                var address3Text = addres2Div.FindElement(By.CssSelector("span:nth-child(1)")).Text;
                Holder holderItem = new Holder();
                holderItem.Name = applicantTitle;
                holderItem.Address1 = addres1;
                holderItem.Address2 = addres2Text;
                holderItem.Address3 = address3Text;
                searchResultItem.Holder = holderItem;

            }
            else if (inidCode == "740")
            {
                var textDiv = pDiv.FindElement(By.CssSelector("div.text"));
                var representetiveName = textDiv.FindElement(By.CssSelector("div:nth-child(1)")).Text;

                searchResultItem.RepresentativeName = representetiveName;
            }
            else if (inidCode == "511")
            {
                IList<GoodsAndServices> goodsAndServices = new List<GoodsAndServices>();

                IWebElement dlElement = driver.FindElement(By.CssSelector("dl.GSGR.BASICGS")); // Adjust the CSS selector as needed

                // Find all <dt> elements within the <dl>
                IList<IWebElement> dtElements = dlElement.FindElements(By.CssSelector("dt.GSGR.BASICGS"));

                // Loop through each <dt> element and process the corresponding <dd> element
                foreach (IWebElement dtElement in dtElements)
                {
                    // Get the text value from <dt> element
                    string dtValue = dtElement.Text.Trim();

                    // Find the corresponding <dd> element for the current <dt>
                    IWebElement ddElement = dtElement.FindElement(By.XPath("following-sibling::dd[1]"));

                    // Get the text value from the corresponding <dd> element
                    string ddValue = ddElement.FindElement(By.CssSelector("p.gsterm.shown.firstLanguage.GSTERMEN")).Text.Trim();

                    goodsAndServices.Add(new GoodsAndServices { Code = dtValue, Description = ddValue });
                }
                searchResultItem.GoodsAndServices = goodsAndServices;
            }

        }

        return searchResultItem;
    }
    public List<SearchResultDetail> ProcessPage()
    {
        List<SearchResultDetail> searchResultItems = new List<SearchResultDetail>();

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

            SearchResultDetail searchResultItem = new SearchResultDetail()
            {
                Brand = brand,
                ImageUrl = imageUrl,
                Status = status,
                RegistrationNo = irn,
                RegistrationDate = rd,
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