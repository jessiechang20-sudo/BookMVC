using Microsoft.AspNetCore.Mvc.Rendering;
using BookMvc.Models.Entities;

namespace BookMvc.Models.ViewModels
{
    public class BookIndexVm
    {
        public List<Book> Books { get; set; } = new();

        public string? Sort { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10; //每頁顯示幾筆資料
        public int TotalCount { get; set; } //查詢後的總筆數
        public int TotalPages
        {
            get
            {
                int size = Math.Max(1, PageSize);
                int totalpages = (int)Math.Ceiling((double)TotalCount / size);
                return Math.Max(1, totalpages);
            }
        }


        public IEnumerable<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();



    }
}
