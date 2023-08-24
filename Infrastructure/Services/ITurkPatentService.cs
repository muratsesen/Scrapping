public interface ITurkPatentService
{
    string GetList(TPSearchInBrandsModel model);
    TPFileSearchResultModel GetDetail(TPSearchInFilesModel model);
}