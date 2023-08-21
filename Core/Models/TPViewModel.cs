namespace Core.Models;


public class TPViewModel
{
    public SearchModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}