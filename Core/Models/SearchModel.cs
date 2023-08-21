public class SearchModel
{
    public string IntRegNo { get; set; }//="1254874";
    public string BaseNo { get; set; }
    public string HolderName { get; set; }// = "beko";
    public string ApplicationNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public string ApplicantInfo { get; set; }
    public string BulletinNumber { get; set; }
    public SearchField SearchField { get; set; }
}

public enum SearchField
{
    BrandMonitoring,
    ApplicationMonitoring
}