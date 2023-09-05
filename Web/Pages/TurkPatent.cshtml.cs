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

            var resultModel = service.GetDetail(model);

            BrandData = resultModel.BrandData;
            GoodsAndServices = resultModel.GoodsAndServices;
            return Page();
        }

        public IActionResult OnPostBrand()
        {
            var items = BrandSearchModel;
            var searchModel = new TPSearchModel();

            List<TPSearchModel> users = new List<TPSearchModel>
            {
               new TPSearchModel
               {
                   UserId = "1",
                   ApplicationOwner = "ülker",
                   CurrentPage = 0,
                   NextPage = 0,
                   TotalPages = 0,
                   SearchType = TPSearchType.SearchInBrands
               },
               new TPSearchModel{
                   UserId = "1",
                   ApplicationOwner = "ülker",
                   CurrentPage = 1,
                   NextPage = 2,
                   TotalPages = 8,
                   SearchType = TPSearchType.SearchInBrands
               },
                new TPSearchModel{
                    UserId = "2",
                   ApplicationOwner = "eti",
                   CurrentPage = 0,
                   NextPage = 0,
                   TotalPages = 0,
                   SearchType = TPSearchType.SearchInBrands
               },
               new TPSearchModel{
                   UserId = "1",
                   ApplicationOwner = "ülker",
                   CurrentPage = 1,
                   NextPage = 2,
                   TotalPages = 8,
                   SearchType = TPSearchType.SearchInBrands
               },
                 new TPSearchModel{
                     UserId = "2",
                   ApplicationOwner = "eti",
                   CurrentPage = 1,
                   NextPage = 2,
                   TotalPages = 8,
                   SearchType = TPSearchType.SearchInBrands
               },

            };

            foreach (var item in users)
            {
                var res = service.Scrape(item);

            }

            //searchModel.UserId = useId;
            //searchModel.ApplicationOwner = BrandSearchModel.ApplicationOwner;
            //searchModel.SearchType = TPSearchType.SearchInBrands;
            //service.Scrape(searchModel);



            return Page();
        }

    }
}
