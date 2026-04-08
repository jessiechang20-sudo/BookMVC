using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookMvc.Models;
using BookMvc.Models.Entities;
using BookMvc.Models.ViewModels;
using BookMvc.Services;
using BookMvc.Models.Inputs;
using BookMvc.Models.Common;

namespace BookMvc.Controllers
{
    public sealed class BooksController : Controller
    {
        private readonly IBookService _svc;
        private readonly ILogger<BooksController> _logger;  
        public BooksController(IBookService svc, ILogger<BooksController> logger)
        {
            _svc = svc;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] BookIndexVm vm, CancellationToken ct)
        {
            vm.Sort ??= "id_desc";
            ViewBag.Sort = vm.Sort;
            Enum.TryParse<SortOption>(vm.Sort, true, out var sort);

            var input = new QueryInput()
            {
                Keyword = vm.Keyword,
                Sort = sort,
                Page = vm.Page,
                PageSize = vm.PageSize
            };


            (vm.Books, vm.TotalCount) = await _svc.GetListAsync(input, ct);

            vm.SortOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "id", Text = "ID↑" },
                    new SelectListItem { Value = "id_desc", Text = "ID↓"},
                    new SelectListItem { Value = "isbn", Text = "ISBN↑" },
                    new SelectListItem { Value = "isbn_desc", Text = "ISBN↓" },
                    new SelectListItem { Value = "title", Text = "書名↑" },
                    new SelectListItem { Value = "title_desc", Text = "書名↓" },
                };

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var book = await _svc.GetByIdAsync(id, ct);
            if (book is null)
            {
                return NotFound();
            }
            return View(book);
        }



        //新增Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new BookCreateVm());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]  //防止 CSRF（跨站請求偽造）
        public async Task<IActionResult> Create(BookCreateVm vm, CancellationToken ct)
        {
            _logger.LogInformation("[BOOK_CREATE] Isbn: {Isbn} , Title: {Title} ,HasFile: {HasFile} ", vm.Isbn, vm.Title, vm.Image != null );
            if (ModelState.IsValid is false) //基本表單驗證
            {
                return View(vm);
            }

            var input = new CreateInput()
            {
                Isbn = vm.Isbn,
                Title = vm.Title,
                Author = vm.Author,
                Image = vm.Image,
            }; 

            var (result , book) = await _svc.CreateAsync(input, ct);
            if (result.Ok is false || book is null)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Field ?? string.Empty, error.Message ?? "未知錯誤");  //顯示邏輯驗證錯誤
                }
                return View(vm);
            }
            return RedirectToAction(nameof(Index));  //新增成功後，重定向到 Index 頁面，讓使用者看到更新後的書籍列表。
        }


        //編輯更新
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var book = await _svc.GetByIdAsync(id, ct);
            if (book is null)
            {
                return NotFound();
            }
            var vm = new BookEditVm
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                OriginalCover = book.BCoverFileName
            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, BookEditVm vm, CancellationToken ct)
        {
            _logger.LogInformation("[BOOK_EDIT] Id: {Id} , Title: {Title} , HasOldFile: {HasOldFile}, HasNewFile: {HasNewFile} ", id, vm.Title , vm.OriginalCover != null, vm.Image !=null );
            if (id != vm.Id) //id從網址來，vm.Id從表單來，兩者要一致才合理
            {
                return BadRequest();
            }

            if (ModelState.IsValid is false) //基本表單驗證
            {
                return View(vm);
            }

            var input = new EditInput()
            {
                Isbn = vm.Isbn,
                Title = vm.Title,
                Author = vm.Author,
                Image = vm.Image,
            };

            var (result , book) = await _svc.UpdateAsync(id, input, ct);
            if (result.Ok is false || book is null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Field ?? string.Empty, error.Message ?? "未知錯誤");  //顯示邏輯驗證錯誤
                }
                return View(vm);
            }

            return RedirectToAction(nameof(Edit), new { id });
        }


        //刪除
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var book = await _svc.GetByIdAsync(id, ct);
            if (book is null)
            {
                return NotFound();
            }
            return View(book);
        }



        [HttpPost, ActionName("Delete")] //路由用Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var ok = await _svc.DeleteAsync(id, ct);
            if (ok is false)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
