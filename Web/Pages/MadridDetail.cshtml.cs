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

        private IMadridService service;

        public MadridDetailModel(IMadridService service)
        {
            this.service = service;
        }


        public void OnGet(string registrationNo)
        {
            RegistrationNo = registrationNo;
            var jsonResponse = service.GetList(new MadridSearchModel { RegistrationNo = registrationNo });

            if (jsonResponse == null) return;

            var searchResultList = JsonSerializer.Deserialize<IEnumerable<Core.Models.SearchResultDetail>>(jsonResponse);
            SearchResultDetail = searchResultList?.FirstOrDefault();
        }
    }
}
