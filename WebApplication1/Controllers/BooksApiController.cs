using BookMvc.Models.DTO;
using BookMvc.Models.Inputs;
using BookMvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace BookMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly IBookService _svc;
        public BooksApiController(IBookService svc)
        {
            _svc = svc;
        }


        /// <summary>
        /// 取得書籍清單
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookListApi([FromQuery] QueryInput input, CancellationToken ct)
        {
            var (books, totalCounts) = await _svc.GetListAsync(input, ct);
            var data = books.Select(book =>
                new QueryDto()
                {
                    Id = book.Id,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    Author = book.Author,
                    CreatedAt = book.CreatedAt.ToOffset(TimeSpan.FromHours(8)),
                    ImgName = book.BCoverFileName,
                }).ToList();
            return Ok(data);

        }


        /// <summary>
        /// 根據Id取得書籍資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdApi(int id, CancellationToken ct)
        {
            var book = await _svc.GetByIdAsync(id, ct);
            if (book == null) { return NotFound(); }
            var data = new QueryDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                CreatedAt = book.CreatedAt.ToOffset(TimeSpan.FromHours(8)),
                ImgName = book.BCoverFileName
            };
            return Ok(data);
        }


        /// <summary>
        /// 新增書籍資料
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateApi([FromForm] CreateInput input, CancellationToken ct)
        {
            var (result, book) = await _svc.CreateAsync(input, ct);
            if (result.Ok is false || book is null)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message ?? "發生錯誤，請稍後再試");
                }
                return ValidationProblem(ModelState);
            }

            var data = new CreateDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                CreatedAt = book.CreatedAt.ToOffset(TimeSpan.FromHours(8)),
                ImgName = book.BCoverFileName
            };

            return CreatedAtAction("GetByIdApi", new { id = book.Id }, data);

        }


        /// <summary>
        /// 編輯書籍資訊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPut("edit/{id:int}")]
        public async Task<IActionResult> EditApi(int id, [FromForm] EditInput input, CancellationToken ct)
        {
            var (result, book) = await _svc.UpdateAsync(id, input, ct);
            if (result.Ok is false || book is null)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message ?? "發生未知錯誤，請稍後再試");
                }
                return ValidationProblem(ModelState);
            }
            var data = new EditDto
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                CreatedAt = book.CreatedAt.ToOffset(TimeSpan.FromHours(8)),
                ImgName = book.BCoverFileName
            };
            return Ok(data);
        }


        /// <summary>
        /// 根據id刪除書籍
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id, CancellationToken ct)
        {
            bool ok = await _svc.DeleteAsync(id, ct);
            if (ok is false)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}

