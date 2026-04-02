using System.ComponentModel.DataAnnotations;

namespace BookMvc.Models.Inputs
{
    public class CreateInput
    {
        /// <summary>
        /// ISBN，如:9781982137274
        /// </summary>
        [Required(ErrorMessage = "請輸入ISBN")]
        [StringLength(20)]
        public string Isbn { get; set; } = default!;

        /// <summary>
        /// 書名
        /// </summary>
        [Required(ErrorMessage = "請輸入書名")]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// 作者
        /// </summary>
        [StringLength(100)]
        public string? Author { get; set; }


        /// <summary>
        /// 書籍封面
        /// </summary>
        public IFormFile? Image { get; set; }
    }
}
