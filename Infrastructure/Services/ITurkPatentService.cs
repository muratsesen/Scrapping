public interface ITurkPatentService
{
    string GetList(SearchModel model);
    string GetDetail(SearchModel model);
}