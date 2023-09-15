using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.Json;
using Core.Models;
using Infrastructure.Services.Abstract;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;
public class TurkPatentService : ITurkPatentService
{
    #region Properties
    private string Url = "https://www.turkpatent.gov.tr/arastirma-yap?form=trademark";

    public IWebDriver driver { get; set; }
    public IDriverContainer driverContainer { get; set; }
    private IWebDriver brandSearchDriver { get; set; }

    public TurkPatentService(IDriverContainer container, IWebDriver webDriver)
    {
        driverContainer = container;
        brandSearchDriver = webDriver;
        SetDriver(brandSearchDriver);
    }

    private void SetDriver(IWebDriver webDriver)
    {
        webDriver.Navigate().GoToUrl(Url);

        webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        var tabButtons = webDriver.FindElements(By.CssSelector("button.MuiButtonBase-root.MuiTab-root.MuiTab-textColorInherit.jss32"));

        var buttonFileSearch = tabButtons[1];

        buttonFileSearch.Click();
    }
    #endregion

    #region  Brand Search
    public TPBrandResultModel ScrapeBrandList(TPSearchModel model)
    {
        #region Setting Driver

        var requestedDriver = driverContainer.GetDriver(model.UserId, Url);

        if (requestedDriver == null)
            return null;//"error";

        this.driver = requestedDriver;

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        #endregion

        #region Yeni arama mı devam eden mi

        #region Devam Eden Arama
        if (model.TotalPages > 0 && model.NextPage > model.CurrentPage)
        {
            //* Sonraki sayfayı getir
            if (model.NextPage > model.TotalPages) return null;

            // Butonu bul:  button title = Next page aria-label=Sonraki
            try
            {
                var nextPageButton = driver.FindElement(By.CssSelector("button[title='Next page']"));

                nextPageButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                // Belirtilen özelliklere sahip buton bulunamadığında hata ele alınır.
                Console.WriteLine("Sonraki sayfa butonu bulunamadı." + ex.Message);
                return null;//Hata dön
            }

            //3 saniye bekle
            Thread.Sleep(3000);//TODO refactor

            return ProcessTable();
        }
        #endregion

        #region Yeni Arama

        //Inputları yaz
        try
        {
            var tabButtons = driver.FindElements(By.CssSelector("button.MuiButtonBase-root.MuiTab-root.MuiTab-textColorInherit.jss32"));

            var buttonBrandSearch = tabButtons[0];

            buttonBrandSearch.Click();

            //find input fields with class names MuiInputBase-input MuiInput-input
            var inputElements = driver.FindElements(By.CssSelector("input.MuiInputBase-input.MuiInput-input"));

            //fill input fields
            if (model.ApplicationOwner != null) inputElements[1].SendKeys(model.ApplicationOwner);
            if (model.BrandAdvertisementBulletinNumber != null) inputElements[2].SendKeys(model.BrandAdvertisementBulletinNumber);
            if (model.IndividualNumber != null) inputElements[3].SendKeys(model.IndividualNumber);

            //Sorgula butonunu bul
            var searchButton = driver.FindElement(By.CssSelector("button.MuiButtonBase-root.MuiButton-root.MuiButton-contained.MuiButton-containedSecondary"));

            searchButton.Click();
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("Yeni sorgu için inputları girerken hata." + ex.Message);
        }

        //3 saniye bekle
        Thread.Sleep(3000);//TODO refactor

        return ProcessTable();

        #endregion
        #endregion
    }



    private TPBrandResultModel ProcessTable()
    {
        TPBrandResultModel resultModel = new TPBrandResultModel();
        resultModel.BrandDataList = new List<TPBrandData>();

        #region Kayıt sayısını getir
        try
        {
            // IWebElement tableContainer = driver.FindElement(By.XPath("//main/div[2]/div/div[2]"));

            IWebElement searchResultContainer = driver.FindElement(By.Id("search-results"));

            if (searchResultContainer.Text == "Sonuç bulunamadı") return null;

            IWebElement tableContainer = searchResultContainer.FindElement(By.XPath("//div[2]"));


            IWebElement recordCountElement = tableContainer.FindElement(By.XPath("//p"));

            ExtractPageCount(resultModel, recordCountElement.Text);


            var tableBodyElement = tableContainer.FindElement(By.XPath("//div/div/table/tbody"));

            var rows = tableBodyElement.FindElements(By.TagName("tr"));

            foreach (var row in rows)
            {
                var brandData = new TPBrandData();

                var tds = row.FindElements(By.TagName("td"));

                resultModel.BrandDataList.Add(new TPBrandData
                {
                    BrandName = tds[2].Text,
                    ApplicationNo = tds[1].Text

                });
            }

        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("Kayıt sayısını gösteren eleman bulunamadı" + ex.Message);
            return null;
        }

        #endregion

        return resultModel;
    }

    private void ExtractPageCount(TPBrandResultModel model, string metin)
    {
        string sayfaDeseni = @"Sayfa (\d+) \/ (\d+)";

        // Metindeki tüm sayıları alalım
        Match sayfaEslesme = Regex.Match(metin, sayfaDeseni);

        if (sayfaEslesme.Success)
        {
            // Sayfa numarasını ve toplam sayfa sayısını alalım
            int sayfaNumarasi = 0;
            int.TryParse(sayfaEslesme.Groups[1].Value, out sayfaNumarasi);

            int toplamSayfaSayisi = 0;
            int.TryParse(sayfaEslesme.Groups[2].Value, out toplamSayfaSayisi);

            model.CurrentPage = sayfaNumarasi;
            model.TotalPages = toplamSayfaSayisi;
        }
        else
        {
            Console.WriteLine("Metinde beklenen desenler bulunamadı.");
            model.TotalPages = 0;
            model.CurrentPage = 0;
        }

    }

    private TPFileSearchResultModel ProcessRow()
    {
        return null;
    }
    #endregion

    #region  File Detail
    public TPFileSearchResultModel ScrapeFileDetail(TPSearchModel model)
    {

        //brand tab 
        var inputElements = brandSearchDriver.FindElements(By.CssSelector("input.MuiInputBase-input.MuiInput-input"));

        //inputları temizle
        foreach (var inputElement in inputElements)
        {
            inputElement.Clear();
        }

        //inputları gir
        if (model.ApplicationNumber != null) inputElements[0].SendKeys(model.ApplicationNumber);
        if (model.RegistrationNumber != null) inputElements[1].SendKeys(model.RegistrationNumber);
        if (model.ApplicantInfo != null) inputElements[2].SendKeys(model.ApplicantInfo);
        if (model.BulletinNumber != null) inputElements[3].SendKeys(model.BulletinNumber);

        //find search button
        var searchButton = brandSearchDriver.FindElement(By.CssSelector("button.MuiButtonBase-root.MuiButton-root.MuiButton-contained.MuiButton-containedSecondary"));

        searchButton.Click();


        var fieldsets = GetFieldsets();

        IWebElement brandInfoElement = null;
        IWebElement goodsElemets = null;

        if (fieldsets.Count >= 2)
        {
            // Assign the first fieldset to brandInfoElement
            brandInfoElement = fieldsets.First();

            // Assign the second fieldset to goodsElemets
            goodsElemets = fieldsets.ElementAt(1);
        }

        #region Brand Info
        //Marka logo
        var logoUrl = GetImage(brandInfoElement);

        //Diğer dataları tbody içinden çekeceğiz
        var brandInfoTableRows = GetTableRowsInFieldset(brandInfoElement);

        (string, string) tupleApplcaitonNoAndDate = ApplicaitonNoAndDate(brandInfoTableRows.First());
        (string, string) registrationNoAndDate = GetRegistrationNoDate(brandInfoTableRows.ElementAt(1));
        (string, string) verdictAndReason = GetVerdictAndReason(brandInfoTableRows.ElementAt(11));

        TPBrandData brandData = new TPBrandData
        {
            LogoUrl = GetImage(brandInfoElement),
            ApplicationNo = tupleApplcaitonNoAndDate.Item1,
            ApplicationDate = tupleApplcaitonNoAndDate.Item2,
            RegistrationNo = registrationNoAndDate.Item1,
            RegistrationDate = registrationNoAndDate.Item2,
            InternationalRegistrationNo = GetInternationalRegistrationNo(brandInfoTableRows.ElementAt(2)),
            PublicationDate = GetPublicationDate(brandInfoTableRows.ElementAt(3)),
            PublicationNo = GetPublicationNo(brandInfoTableRows.ElementAt(4)),
            State = GetState(brandInfoTableRows.ElementAt(5)),
            RucHan = GetRuchan(brandInfoTableRows.ElementAt(6)),
            NiceClasses = GetNiceClasses(brandInfoTableRows.ElementAt(7)),
            Type = GetType(brandInfoTableRows.ElementAt(7)),
            BrandName = GetBrandName(brandInfoTableRows.ElementAt(8)),
            Owner = GetOwnerInfo(brandInfoTableRows.ElementAt(10)),
            Verdict = verdictAndReason.Item1,
            VerdictReason = verdictAndReason.Item2
        };

        #endregion


        #region Goods And Elements

        var goodsTableRows = GetTableRowsInFieldset(brandInfoElement);

        List<TPGoodsAndService> goodsAndServices = new List<TPGoodsAndService>();

        foreach (var row in goodsTableRows)
        {
            var tds = row.FindElements(By.TagName("td"));

            if (tds.Count >= 2)
            {
                var classCode = tds[0].Text.Trim();
                var content = tds[1].Text.Trim();

                goodsAndServices.Add(new TPGoodsAndService
                {
                    ClassCode = classCode,
                    Content = content
                });
            }
        }

        #endregion

        TPFileSearchResultModel result = new TPFileSearchResultModel();
        result.BrandData = brandData;
        result.GoodsAndServices = goodsAndServices;
        return result;
    }

    IReadOnlyCollection<IWebElement> GetFieldsets()
    {
        try
        {
            return brandSearchDriver.FindElements(By.TagName("fieldset"));
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("Element not found: " + e.Message);
        }
        return null;
    }

    #region Brand Info Methods
    string GetImage(IWebElement fieldsetElement)
    {
        try
        {
            // Locate the img element
            IWebElement imgElement = fieldsetElement.FindElement(By.CssSelector("img"));

            // Get the value of the src attribute
            string srcAttributeValue = imgElement.GetAttribute("src");

            return srcAttributeValue;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("Image element not found: " + e.Message);
        }
        return "";
    }

    IReadOnlyCollection<IWebElement> GetTableRowsInFieldset(IWebElement fieldsetElement)
    {
        try
        {
            var tbody = fieldsetElement.FindElement(By.CssSelector("tbody"));
            var rows = tbody.FindElements(By.TagName("tr"));

            return rows;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return null;
    }
    (string, string) ApplicaitonNoAndDate(IWebElement element)
    {
        try
        {
            // Locate the specific td element
            var tdElements = element.FindElements(By.TagName("td"));

            if (tdElements.Count >= 4)
            {
                return (tdElements.ElementAt(1).Text, tdElements.ElementAt(3).Text);
            }

        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return ("", "");
    }
    (string, string) GetRegistrationNoDate(IWebElement element)
    {
        try
        {
            // Locate the specific td element
            var tdElements = element.FindElements(By.TagName("td"));

            if (tdElements.Count >= 4)
            {
                return (tdElements.ElementAt(1).Text, tdElements.ElementAt(3).Text);
            }

        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return ("", "");
    }

    string GetInternationalRegistrationNo(IWebElement element)
    {
        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetPublicationDate(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetPublicationNo(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetState(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:last-child"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetRuchan(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:last-child"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetNiceClasses(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetType(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:last-child"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetBrandName(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    string GetOwnerInfo(IWebElement element)
    {

        try
        {
            IWebElement tdElement = element.FindElement(By.CssSelector("tr > td:last-child"));
            return tdElement.Text;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return "";
    }

    (string, string) GetVerdictAndReason(IWebElement element)
    {
        try
        {
            IWebElement tdElement1 = element.FindElement(By.CssSelector("tr > td:nth-child(2)"));
            IWebElement tdElement2 = element.FindElement(By.CssSelector("tr > td:last-child"));
            return (tdElement1.Text, tdElement2.Text);
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine("TD element not found: " + e.Message);
        }
        return ("", "");
    }

    #endregion
    #endregion
}