using Microsoft.AspNetCore.Mvc.Rendering;
using BookMvc.Models.Entities;
using BookMvc.Models.Common;
using System.ComponentModel;

namespace BookMvc.Models.Inputs
{
    public class QueryInput
    {
        /// <summary>
        /// 關鍵字，可支援搜尋ISBN、書名、作者...等
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public SortOption? Sort { get; set; }

        /// <summary>
        /// 頁數
        /// </summary>
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每頁筆數
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10; //每頁顯示幾筆資料
    }

}
