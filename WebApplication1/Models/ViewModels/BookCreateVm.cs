using System.ComponentModel.DataAnnotations;  

namespace WebApplication1.Models.ViewModels
{
    public sealed class BookCreateVm  
    {
        [Required]
        [StringLength(20)]
        public string Isbn { get; set; } = default!; 

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(100)]
        public string? Author { get; set; } 

        public IFormFile? NewCover { get; set; }
    }
}
