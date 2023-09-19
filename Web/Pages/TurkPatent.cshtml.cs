using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages
{
    public class TurkPatentModel : PageModel
    {
        [BindProperty]
        public TPSearchInFilesModel SearchFilesModel { get; set; }

        [BindProperty]
        public TPSearchInBrandsModel BrandSearchModel { get; set; }

        [BindProperty]
        public string useId { get; set; }


        public TPBrandData BrandData { get; set; }
        public List<TPGoodsAndService> GoodsAndServices { get; set; }

        private ITurkPatentService service;

        public TurkPatentModel(ITurkPatentService service)
        {
            this.service = service;
            SearchFilesModel = new TPSearchInFilesModel();
            SearchFilesModel.ApplicantInfo = "";
            SearchFilesModel.ApplicationNumber = "";
            SearchFilesModel.BulletinNumber = "";
            SearchFilesModel.RegistrationNumber = "";

        }

        public void OnGet()
        {
        }

        public IActionResult OnPostFiles()
        {
            var items = SearchFilesModel;

            TPSearchModel model = new TPSearchModel();
            model.SearchType = TPSearchType.ChaseFile;

            model.ApplicationNumber = SearchFilesModel.ApplicationNumber;
            model.RegistrationNumber = SearchFilesModel.RegistrationNumber;
            model.ApplicantInfo = SearchFilesModel.ApplicantInfo;
            model.BulletinNumber = SearchFilesModel.BulletinNumber;

            var resultModel = service.ScrapeFileDetail(model);

            BrandData = resultModel.BrandData;
            GoodsAndServices = resultModel.GoodsAndServices;
            return Page();
        }

        public IActionResult OnPostBrand()
        {
            var items = BrandSearchModel;
            var searchModel = new TPSearchModel();

            List<TPTestModel> test = new List<TPTestModel>
            {
                //Başarısız arama
                new TPTestModel
                {
                    search =  new TPSearchModel
                    {
                        UserId = "1",
                        ApplicationOwner = "ülkerrrrrr",
                        CurrentPage = 0,
                        NextPage = 0,
                        TotalPages = 0,
                        SearchType = TPSearchType.SearchInBrands
                    },
                    BrandResult = new TPBrandResultModel(),
                    FileResult = new TPFileSearchResultModel()
                },
                //kullanıcı 1 başarılı arama
                 new TPTestModel
                 {
                    search =  new TPSearchModel
                    {
                        UserId = "1",
                        ApplicationOwner = "ülker",//yeni arama
                        CurrentPage = 0,
                        NextPage = 0,
                        TotalPages = 0,
                        SearchType = TPSearchType.SearchInBrands
                    },
                    BrandResult = new TPBrandResultModel(),
                    FileResult = new TPFileSearchResultModel()
                },
                //kullanıcı 1 devam arama
                 new TPTestModel
                 {
                    search =  new TPSearchModel
                    {
                        UserId = "1",
                        ApplicationOwner = "ülker",//yeni arama
                        CurrentPage = 1,
                        NextPage = 2,
                        TotalPages = 10,
                        SearchType = TPSearchType.SearchInBrands
                    },
                    BrandResult = new TPBrandResultModel(),
                    FileResult = new TPFileSearchResultModel()
                },
                //kullanıcı 2 başarılı arama
                 new TPTestModel
                 {
                    search =  new TPSearchModel
                    {
                        UserId = "2",
                        ApplicationOwner = "arçelik",
                        CurrentPage = 0,
                        NextPage = 0,
                        TotalPages = 0,
                        SearchType = TPSearchType.SearchInBrands
                    },
                    BrandResult = new TPBrandResultModel(),
                    FileResult = new TPFileSearchResultModel()
                },
                //kullanıcı 2 devam arama
                 new TPTestModel
                 {
                    search =  new TPSearchModel
                    {
                        UserId = "2",
                        ApplicationOwner = "arçelik",
                        CurrentPage = 1,
                        NextPage = 2,
                        TotalPages = 10,
                        SearchType = TPSearchType.SearchInBrands
                    },
                    BrandResult = new TPBrandResultModel(),
                    FileResult = new TPFileSearchResultModel()
                }
            };

            foreach (var item in test)
            {
                item.BrandResult = service.ScrapeBrandList(item.search);

                if (item.BrandResult != null)
                {

                    item.FileResult = service.ScrapeFileDetail(new TPSearchModel
                    {
                        ApplicationNumber = item.BrandResult.BrandDataList[0].ApplicationNo
                    });
                }

            }

            //searchModel.UserId = useId;
            //searchModel.ApplicationOwner = BrandSearchModel.ApplicationOwner;
            //searchModel.SearchType = TPSearchType.SearchInBrands;
            //service.Scrape(searchModel);



            return Page();
        }

    }
}
