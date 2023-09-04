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
    public class MadridModel : PageModel
    {
        [BindProperty]
        public WipoSearchModel SearchModel { get; set; }

        [BindProperty]
        public IEnumerable<SearchResultDetail>? SearchResults { get; set; }


        private IWipoService service;

        public MadridModel(IWipoService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
            SearchModel = new WipoSearchModel();
        }

        public IActionResult OnPost()
        {
            var (searchResult, isSingleItem) = service.GetList(SearchModel);

            if (searchResult == null)
            {
                // Handle the case where searchResult is null
                return Page();
            }

            if (isSingleItem)
            {
                var serializedList = JsonSerializer.Serialize(searchResult.Details.ToList());
                TempData["SearchResultList"] = serializedList;
                return RedirectToPage("MadridDetail");
            }

            SearchResults = searchResult.Details;
            SearchModel.TotalPages = searchResult.TotalPages;
            SearchModel.CurrentPage = searchResult.CurrentPage;

            return Page();
        }


    }
}
