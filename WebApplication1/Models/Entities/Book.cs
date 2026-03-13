using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities
{
    public sealed class Book
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Isbn { get; set; } = default!;

        [MaxLength(200)]
        public string Title { get; set; } = default!;

        [MaxLength(100)]
        public string? Author { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string? BCoverFileName { get; set; } // 例如： "abc.jpg"書籍封面圖片檔名，後續使用GUID存檔名，避免檔名衝突
    }
}
