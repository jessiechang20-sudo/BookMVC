using System.ComponentModel.DataAnnotations; 

namespace BookMvc.Models.ViewModels
{
    public class BookEditVm
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="請輸入ISBN")]
        [StringLength(20)]  
        public string Isbn { get; set; } = default!;

        [Required(ErrorMessage = "請輸入書名")]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(100)]
        public string? Author { get; set; }

        public IFormFile? Image { get; set; } //使用者新上傳的書籍封面圖片
        public string? OriginalCover { get; set; } //原本的封面圖片檔名

    }
}
