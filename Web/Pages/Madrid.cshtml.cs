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
        public MadridSearchModel SearchModel { get; set; }

        [BindProperty]
        public IEnumerable<SearchResultDetail>? SearchResults { get; set; }


        private IMadridService service;

        public MadridModel(IMadridService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
            SearchModel = new MadridSearchModel();
        }

        public IActionResult OnPost()
        {
            var (searchResultList, isSingleItem) = service.GetList(SearchModel);

            if (searchResultList == null)
            {
                // Handle the case where searchResultList is null
                return Page();
            }

            if (isSingleItem)
            {
                var serializedList = JsonSerializer.Serialize(searchResultList.ToList());
                TempData["SearchResultList"] = serializedList;
                return RedirectToPage("MadridDetail");
            }

            SearchResults = searchResultList;

            return Page();
        }


    }
}
