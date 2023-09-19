using System.ComponentModel.DataAnnotations;
using Core.Models;

public class WipoSearchModel:SearchBase
{
    public string RegistrationNo { get; set; }//="1254874";
    public string BaseNo { get; set; }
    public string HolderName { get; set; }// = "beko";
    public string UserId { get; set; }
}
public class TPSearchInFilesModel
{
    [Display(Name = "Başvuru Numarası")]
    public string ApplicationNumber { get; set; } = "2023/023132";
    [Display(Name = "Evrak Numarası")]
    public string RegistrationNumber { get; set; } = "";
    [Display(Name = "Tescil Numarası")]
    public string ApplicantInfo { get; set; } = "";
    [Display(Name = "Uluslararası Tescil Numarası")]
    public string BulletinNumber { get; set; } = "";
}
public class TPSearchInBrandsModel
{
    [Display(Name = "Başvuru Sahibi")]
    public string ApplicationOwner { get; set; }
    [Display(Name = "Marka İlan Bülten No")]
    public string BrandAdvertisementBulletinNumber { get; set; }
    [Display(Name = "Kişi Numarası")]
    public string IndividualNumber { get; set; }
}

public enum TPSearchType
{
    ChaseFile,
    SearchInBrands
}
public class TPSearchModel : SearchBase
{
    public string UserId { get; set; }
    public TPSearchType SearchType { get; set; }
    [Display(Name = "Başvuru Numarası")]
    public string ApplicationNumber { get; set; } = "2023/023132";
    [Display(Name = "Evrak Numarası")]
    public string RegistrationNumber { get; set; } = "";
    [Display(Name = "Tescil Numarası")]
    public string ApplicantInfo { get; set; } = "";
    [Display(Name = "Uluslararası Tescil Numarası")]
    public string BulletinNumber { get; set; } = "";

    [Display(Name = "Başvuru Sahibi")]
    public string ApplicationOwner { get; set; }
    [Display(Name = "Marka İlan Bülten No")]
    public string BrandAdvertisementBulletinNumber { get; set; }
    [Display(Name = "Kişi Numarası")]
    public string IndividualNumber { get; set; }
}
