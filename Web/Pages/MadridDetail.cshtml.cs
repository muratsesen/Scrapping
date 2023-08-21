using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class MadridDetailModel : PageModel
    {
        public string IntRegNo { get; set; }
        public string BaseNo { get; set; }
        public string HolderName { get; set; }

        public void OnGet(string intRegNo)
        {
            IntRegNo = intRegNo;
            // You can use these parameters to fetch and display the detailed information
        }
    }
}
