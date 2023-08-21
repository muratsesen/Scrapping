namespace Core.Models;


public class MadridViewModel
{
    public MadridSearchModel SearchModel { get; set; }
    public IEnumerable<SearchResultItem>? SearchResults { get; set; }
}