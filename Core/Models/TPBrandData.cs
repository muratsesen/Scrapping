public class TPFileSearchResultModel
{
    public TPBrandData BrandData { get; set; }
    public List<TPGoodsAndService> GoodsAndServices { get; set; }
}

public class TPBrandData
{
    public string? LogoUrl { get; set; }
    public string? ApplicationNo { get; set; }
    public string? ApplicationDate { get; set; }
    public string? RegistrationNo { get; set; }
    public string? RegistrationDate { get; set; }
    public string? InternationalRegistrationNo { get; set; }
    public string? PublicationDate { get; set; }
    public string? PublicationNo { get; set; }
    public string? State { get; set; }
    public string? RucHan { get; set; }
    public string? NiceClasses { get; set; }
    public string? Type { get; set; }
    public string? BrandName { get; set; }
    public string? Owner { get; set; }
    public string? Verdict { get; set; }
    public string? VerdictReason { get; set; }
}


public class TPGoodsAndService
{
    public string? ClassCode { get; set; }
    public string? Content { get; set; }
}
