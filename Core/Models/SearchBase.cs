using System;
namespace Core.Models
{
    public class SearchBase
    {
        public int TotalPages { get; set; } = 0;
        public int CurrentPage { get; set; } = 0;
        public int NextPage { get; set; } = 0;
    }
}

