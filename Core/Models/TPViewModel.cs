namespace Core.Models;


public class TPViewModel
{
    public SearchInTPModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}