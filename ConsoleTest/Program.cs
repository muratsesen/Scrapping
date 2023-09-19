// See https://aka.ms/new-console-template for more information
using Infrastructure.Services;
using OpenQA.Selenium.Firefox;

//ITurkPatentService service = new TurkPatentService(new DriverContainer(), new FirefoxDriver());
IWipoService wipoService = new WipoService(new DriverContainer());


var search1 = new TPSearchModel
{
    UserId = "1",
    ApplicationOwner = "arçelik",
    CurrentPage = 0,
    NextPage = 0,
    TotalPages = 0,
    SearchType = TPSearchType.SearchInBrands
};

//service.ScrapeBrandList(search1);

var search2 = new TPSearchModel
{
    UserId = "1",
    ApplicationOwner = "eti",
    CurrentPage = 0,
    NextPage = 0,
    TotalPages = 0,
    SearchType = TPSearchType.SearchInBrands
};

//service.ScrapeBrandList(search2);

var search3 = new WipoSearchModel
{
    RegistrationNo="1254874",
    CurrentPage = 0,
    NextPage = 0,
    TotalPages = 0,
    UserId = "1"
};
wipoService.Scrape(search3);

var search4 = new WipoSearchModel
{
    RegistrationNo="1254874",
    CurrentPage = 0,
    NextPage = 0,
    TotalPages = 0,
    UserId = "2"
};
wipoService.Scrape(search4);