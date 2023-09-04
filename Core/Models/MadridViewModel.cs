namespace Core.Models;


public class WipoViewModel
{
    public WipoSearchModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}