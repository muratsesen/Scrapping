namespace Core.Models;


public class MadridViewModel
{
    public SearchInMadridModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}