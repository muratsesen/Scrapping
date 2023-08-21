using System.Text.Json.Serialization;
public class SearchResultItem
{
    [JsonPropertyName("brand")]
    public string Brand { get; set; }
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("origin")]
    public string Origin { get; set; }
    [JsonPropertyName("holder")]
    public string Holder { get; set; }
    [JsonPropertyName("regNo")]
    public string RegNo { get; set; } = "";
    [JsonPropertyName("regDate")]
    public string RegDate { get; set; } = ""; 
    [JsonPropertyName("niceCI")]
    public string NiceCI { get; set; }
    [JsonPropertyName("viennaCI")]
    public string ViennaCI { get; set; } 

}