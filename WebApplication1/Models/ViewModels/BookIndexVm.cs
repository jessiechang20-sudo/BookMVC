using Microsoft.AspNetCore.Mvc.Rendering; 
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class BookIndexVm
    {
        public List<Book> Books { get; set; } = new();

        public string? Sort { get; set; } 
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1; 
        public int PageSize { get; set; } = 10; //每頁顯示幾筆資料
        public int TotalCount { get; set; } //查詢後的總筆數
        public int TotalPages { get { return ((int)Math.Ceiling((double)TotalCount / PageSize) < 1 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize)) ; }  }


        public IEnumerable<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();



    }
}
