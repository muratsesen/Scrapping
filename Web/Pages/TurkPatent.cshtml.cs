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
    public class TurkPatentModel : PageModel
    {
        [BindProperty]
        public TPViewModel TPModel { get; set; }

        private ITurkPatentService service;

        public TurkPatentModel(ITurkPatentService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
            TPModel = new TPViewModel();
            TPModel.SearchModel = new SearchModel();
        }
        public void OnPost()
        {
            var jsonResponse = service.GetList(TPModel.SearchModel);

            if (jsonResponse == null) return;

            var searchResultList = JsonSerializer.Deserialize<IEnumerable<Core.Models.SearchResultItem>>(jsonResponse);
            TPModel.SearchResults = searchResultList;
        }
    }
}
