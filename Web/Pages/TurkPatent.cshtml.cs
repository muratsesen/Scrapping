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

            var resultModel = service.GetDetail(SearchFilesModel);
            BrandData = resultModel.BrandData;
            GoodsAndServices = resultModel.GoodsAndServices;
            return Page(); 
        }

        public IActionResult OnPostBrand()
        {
            var items = BrandSearchModel;
            service.GetList(BrandSearchModel);
            return Page();
        }

    }
}
