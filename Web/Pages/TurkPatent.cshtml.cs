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

        private ITurkPatentService service;

        public TurkPatentModel(ITurkPatentService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPostFiles()
        {
            // Handle form submission for Search Files
            // Access data using SearchFilesModel
            var items = SearchFilesModel;
            return Page(); // Return Page to stay on the same page
        }

        public IActionResult OnPostBrand()
        {
            // Handle form submission for Brand Search
            // Access data using BrandSearchModel
            var items = BrandSearchModel;
            service.GetList(BrandSearchModel);
            return Page(); // Return Page to stay on the same page
        }

    }
}
