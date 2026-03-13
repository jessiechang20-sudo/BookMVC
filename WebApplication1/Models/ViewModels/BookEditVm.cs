using System.ComponentModel.DataAnnotations; 

namespace WebApplication1.Models.ViewModels
{
    public class BookEditVm
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]  
        public string Isbn { get; set; } = default!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(100)]
        public string? Author { get; set; }

        public IFormFile? NewCover { get; set; } //使用者新上傳的書籍封面圖片
        public string? OriginalCover { get; set; } //原本的封面圖片檔名
        public IFormFile? RemoveCover { get; set; } //勾選「移除封面圖片」
    }
}
