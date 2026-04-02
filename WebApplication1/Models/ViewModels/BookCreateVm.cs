using System.ComponentModel.DataAnnotations;  

namespace BookMvc.Models.ViewModels
{
    public sealed class BookCreateVm  
    {
        [Required(ErrorMessage ="請輸入ISBN")]
        [StringLength(20)]
        public string Isbn { get; set; } = default!; 

        [Required(ErrorMessage ="請輸入書名")]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(100)]
        public string? Author { get; set; } 

        public IFormFile? Image { get; set; }
    }
}
