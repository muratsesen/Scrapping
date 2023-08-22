using System.Text.Json.Serialization;

namespace Core.Models;

public class SearchResultDetail
{
    //Brand
    [JsonPropertyName("brand")]
    public string Brand { get; set; }
    //Image
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }
    //Status
    [JsonPropertyName("status")]
    public string Status { get; set; }
    //Registration Number
    [JsonPropertyName("registrationNo")]
    public string RegistrationNo { get; set; } = "";
    //Registration Date
    [JsonPropertyName("registrationDate")]
    public string RegistrationDate { get; set; } = "";
    //Registration End Date
    [JsonPropertyName("registrationEndDate")]
    public string RegistrationEndDate { get; set; } = "";
    //Holder
    [JsonPropertyName("holder")]
    public Holder Holder { get; set; }
    //Name of Representative
    [JsonPropertyName("representativeName")]
    public string RepresentativeName { get; set; } = "";
    //Class code and goods
    [JsonPropertyName("goodsAndServices")]
    public IList<GoodsAndServices> GoodsAndServices { get; set; }
    //Basic application
    [JsonPropertyName("basicApplication")]
    public string BasicApplication { get; set; } = "";
    //Designations
    [JsonPropertyName("designations")]
    public string Designations { get; set; } = "";

}
public class Holder
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("address1")]
    public string Address1 { get; set; } = "";
    [JsonPropertyName("address2")]
    public string Address2 { get; set; } = "";
    [JsonPropertyName("address3")]
    public string Address3 { get; set; } = "";
}

public class GoodsAndServices
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

}