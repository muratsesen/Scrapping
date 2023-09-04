using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Models;
using System.Text.Json;

namespace Web.Pages
{
    public class MadridDetailModel : PageModel
    {
        public string RegistrationNo { get; set; }
        public string BaseNo { get; set; }
        public string HolderName { get; set; }

        [BindProperty]
        public SearchResultDetail SearchResultDetail { get; set; }

        private IWipoService service;

        public MadridDetailModel(IWipoService service)
        {
            this.service = service;
        }

        public IActionResult OnGet()
        {
            var serializedList = TempData["SearchResultList"] as string;
            var searchResultList = JsonSerializer.Deserialize<List<Core.Models.SearchResultDetail>>(serializedList);

            if (searchResultList == null || searchResultList.Count == 0) return Page();

            SearchResultDetail = searchResultList.FirstOrDefault();

            return Page();

        }

        public IActionResult OnPost(string registrationNo)
        {
            RegistrationNo = registrationNo;
            var (searchResultList, isSingleItem) = service.GetList(new WipoSearchModel { RegistrationNo = registrationNo });

            if (searchResultList == null) Page();

            SearchResultDetail = searchResultList.Details.FirstOrDefault();

            return Page();

        }
    }
}
