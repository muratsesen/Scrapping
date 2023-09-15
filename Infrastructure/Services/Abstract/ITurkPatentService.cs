public interface ITurkPatentService
{
    TPBrandResultModel ScrapeBrandList(TPSearchModel model);
    TPFileSearchResultModel ScrapeFileDetail(TPSearchModel model);
}