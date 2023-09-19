using Core.Models;

public interface IWipoService
{
    WipoSearchResult Scrape(WipoSearchModel model);
}