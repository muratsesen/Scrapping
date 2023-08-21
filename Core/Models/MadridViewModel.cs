namespace Core.Models;


public class MadridViewModel
{
    public SearchModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}