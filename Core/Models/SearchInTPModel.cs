public class SearchInTPModel
{
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