public interface ITurkPatentService
{
    TPFileSearchResultModel Scrape(TPSearchModel model);
    TPFileSearchResultModel GetDetail(TPSearchModel model);
}