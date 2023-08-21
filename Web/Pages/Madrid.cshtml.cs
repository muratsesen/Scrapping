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
        public MadridViewModel MVModel { get; set; }

        public void OnGet()
        {
            MVModel = new MadridViewModel();
            MVModel.SearchModel = new SearchInMadridModel();
        }
        public void OnPost()
        {
            Console.WriteLine("reg no" + MVModel.SearchModel.IntRegNo);
            var jsonResponse = MadridService.GetList(MVModel.SearchModel.IntRegNo, MVModel.SearchModel.BaseNo, MVModel.SearchModel.HolderName);

            if (jsonResponse == null) return;

            var searchResultList = JsonSerializer.Deserialize<IEnumerable<Core.Models.SearchResultItem>>(jsonResponse);
            MVModel.SearchResults = searchResultList;
        }
    }
}
