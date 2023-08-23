public interface IMadridService
{
    (IEnumerable<Core.Models.SearchResultDetail> list, bool singleItem) GetList(MadridSearchModel model);
}