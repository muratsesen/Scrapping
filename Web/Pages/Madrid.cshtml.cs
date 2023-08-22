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

        public void OnPost()
        {
            var jsonResponse = service.GetList(SearchModel);

            if (jsonResponse == null) return;

            var searchResultList = JsonSerializer.Deserialize<IEnumerable<Core.Models.SearchResultDetail>>(jsonResponse);
            SearchResults = searchResultList;
        }


    }
}
