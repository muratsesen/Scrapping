using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.Json;
using Core.Models;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using SeleniumExtras.WaitHelpers;

namespace Infrastructure.Services;
public class WipoService : IWipoService
{
    private readonly IWebDriver driver;

    public object ExpectedConditions { get; private set; }

    public WipoService(IWebDriver _driver)
    {
        driver = _driver;
    }

    public (WipoSearchResult, bool) GetList(WipoSearchModel model)
    {

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        try
        {
            driver.Navigate().GoToUrl("https://www3.wipo.int/madrid/monitor/en/");

        }
        catch (Exception ex)
        {
            driver.Quit();
            return (null, false);
        }
        // Wait for the page to load
        //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        //// Define the condition to wait for (e.g., an element that appears when the page is loaded)
        //By condition = By.Id("some-element-id"); // Replace with the actual element ID or another suitable condition

        //// Wait until the condition is met
        //wait.Until(ExpectedConditions.Element(condition));

        //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);


        //-----
        //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        //wait.PollingInterval = TimeSpan.FromMilliseconds(200); wait.Until(ExpectedConditions.ElementIsVisible(By.Id("my-element']"))).Click();
        //------
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

        var result = ProcessResult(model.CurrentPage);
        // driver.Close();

        return result;
    }

    public (WipoSearchResult, bool) ProcessResult(int page = 1)
    {
        IWebElement pageCountElement = driver.FindElement(By.ClassName("pageCount"));

        string pageCountText = pageCountElement.Text;

        WipoSearchResult wipoSearchResult = new WipoSearchResult();
        wipoSearchResult.TotalPages = 1;
        wipoSearchResult.CurrentPage = page;
        wipoSearchResult.Details = new List<SearchResultDetail>();

        if (string.IsNullOrEmpty(pageCountText))
            return (wipoSearchResult, false); // No data

        var pageCount = ExtractNumber(pageCountText);

        if (pageCount == -1) return (null, false); // No data

        if (pageCount == 1)
        {
            var rowCount = RowCount();//tek satır olsa da iki geliyor çünkü görünmeyen bir satır var
            if (rowCount == 0) return (null, false); // No data

            if (rowCount == 2)
            {
                //Tek satır veri var. Detayını dönebilir
                SearchResultDetail detail = ProcessPageWithDetail();

                wipoSearchResult.Details.Add(detail);

                return (wipoSearchResult, true);
            }
        }

        //Buraya geldiyse birden çok satır var. Çok elemanlı liste dönecek.
        wipoSearchResult.TotalPages = pageCount;

        if (page == 1)
        {
            //MEvcut sayfayı işle
            var pageData = ProcessPage();

            wipoSearchResult.Details.AddRange(pageData);
        }
        else if (page <= pageCount)
        {
            //istenen sayfaya git
            try
            {
                IWebElement skipInput = driver.FindElement(By.CssSelector("input#skipValue0"));
                skipInput.Clear();
                skipInput.SendKeys(page.ToString());
                skipInput.SendKeys(Keys.Enter);
            }
            catch { }

            //int loop = 1000;
            //while (loop-- > 0)
            //    try
            //    {
            //        IWebElement pagerPosElement = driver.FindElement(By.CssSelector("div.pagerPos"));
            //        var pagerPosText = pagerPosElement.Text;
            //        break;
            //    }
            //    catch (StaleElementReferenceException e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //TODO refactor
            Thread.Sleep(5000);
            var pageData = ProcessPage();

            wipoSearchResult.Details.AddRange(pageData);
        }
        return (wipoSearchResult, false);
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
        string status = "Active";
        try
        {
            statusDiv = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_STATUS']"));
            IWebElement statusElement = statusDiv.FindElement(By.CssSelector(".status_icon div"));

            // Get the text content of the element
            string statusText = statusElement.Text;

            // Check if the text contains "Inactive"
            if (statusText.Contains("Inactive"))
            {
                status = "Inactive";
            }
        }
        catch (NoSuchElementException) { }

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
        try { irnElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IRN']")); } catch (NoSuchElementException) { }

        string brand = brandElement != null ? brandElement.Text : "";
        string imageUrl = srcAttributeValue;
        string irn = irnElement != null ? irnElement.Text : "";

        SearchResultDetail searchResultItem = new SearchResultDetail()
        {
            Brand = brandElement != null ? brandElement.Text : "",
            ImageUrl = srcAttributeValue,
            Status = status,
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
        var requestedCodes = new string[] { "732", "740", "511", "821", "832", "834" };
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
            else if (inidCode == "821")
            {
                IWebElement divElement = pDiv.FindElement(By.CssSelector("div.text"));

                // Extract and print the desired text
                string extractedText = divElement.Text;
                searchResultItem.BasicApplication = extractedText.Replace("\"", "");
            }
            else if (inidCode == "832")
            {
                IWebElement divElement = pDiv.FindElement(By.CssSelector("div.text.designations"));

                var spanElements = divElement.FindElements(By.TagName("span"));

                searchResultItem.Designations832 = new List<string>();
                foreach (var spanElement in spanElements)
                {
                    string text = spanElement.Text;
                    searchResultItem.Designations832.Add(text);
                }
            }
            else if (inidCode == "834")
            {
                IWebElement divElement = pDiv.FindElement(By.CssSelector("div.text.designations"));

                var spanElements = divElement.FindElements(By.TagName("span"));

                searchResultItem.Designations834 = new List<string>();
                foreach (var spanElement in spanElements)
                {
                    string text = spanElement.Text;
                    searchResultItem.Designations834.Add(text);
                }
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

        var tableV = table.Text;
        var tabletb = tableBody.Text;
        var tabler = rows[1].Text;
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

            try
            {
                brandElement = row.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_BRAND']"));
            }
            catch (NoSuchElementException) { }

            //try
            //{
            //    IWebElement imgTdElement = driver.FindElement(By.CssSelector("[aria-describedby='gridForsearch_pane_IMG']"));
            //    IWebElement imgElement = imgTdElement.FindElement(By.TagName("img"));
            //    srcAttributeValue = imgElement.GetAttribute("src");
            //}
            //catch (NoSuchElementException ex)
            //{
            //    System.Console.WriteLine("Eleman bulunamadı");
            //}

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


}