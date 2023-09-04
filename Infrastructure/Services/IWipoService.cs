using Core.Models;

public interface IWipoService
{
    (WipoSearchResult, bool) GetList(WipoSearchModel model);
}