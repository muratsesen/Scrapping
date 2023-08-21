﻿using System;
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

        private IMadridService service;

        public MadridModel(IMadridService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
            MVModel = new MadridViewModel();
            MVModel.SearchModel = new MadridSearchModel();
        }

        public void OnPost()
        {
            Console.WriteLine("reg no" + MVModel.SearchModel.IntRegNo);
            var jsonResponse = service.GetList(MVModel.SearchModel);

            if (jsonResponse == null) return;

            var searchResultList = JsonSerializer.Deserialize<IEnumerable<Core.Models.SearchResultItem>>(jsonResponse);
            MVModel.SearchResults = searchResultList;
        }


    }
}
