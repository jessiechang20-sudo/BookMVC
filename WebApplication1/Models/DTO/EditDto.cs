using System.ComponentModel.DataAnnotations;

namespace BookMvc.Models.DTO
{
    public class EditDto
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Isbn { get; set; } = default!;

        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(100)]
        public string? Author { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string? ImgName { get; set; } // 例如： "abc.jpg"書籍封面圖片檔名，後續使用GUID存檔名，避免檔名衝突
    }
}
