using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Numerics;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services.Resaults;



namespace WebApplication1.Services
{
    public sealed class BookService : IBookService 
    {
        private readonly BookDbContext _db; 
        private readonly IBCoverStorage _bCoverStorage;
        public BookService(BookDbContext db, IBCoverStorage bCoverStorage)
        {
            _db = db;
            _bCoverStorage = bCoverStorage;
        }

        //查詢
        public async Task<(List<Book> books, int totalcount)> GetListAsync(BookIndexVm vm, CancellationToken ct)
        {
            var query = _db.Books.AsNoTracking(); 


            // (1)Search
            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                vm.Keyword = vm.Keyword.Trim();
                query = query.Where(x => x.Isbn.Contains(vm.Keyword) || x.Title.Contains(vm.Keyword) || (x.Author != null && x.Author.Contains(vm.Keyword)));
            }


            // (2)Sort
            query = vm.Sort switch
            {
                "id" => query.OrderBy(x => x.Id),
                "id_desc" => query.OrderByDescending(x => x.Id),

                "isbn" => query.OrderBy(x => x.Isbn),
                "isbn_desc" => query.OrderByDescending(x => x.Isbn),

                "title" => query.OrderBy(x => x.Title),
                "title_desc" => query.OrderByDescending(x => x.Title),

                _ => query.OrderByDescending(x => x.Id)
            };

            // (3)Page
            vm.Page = vm.Page < 1 ? 1 : vm.Page;
            vm.TotalCount = await query.CountAsync(ct); 
            vm.Page = vm.TotalPages < vm.Page ? vm.TotalPages : vm.Page;
            var books = await query.Skip((vm.Page - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync(ct); 

            return (books, vm.TotalCount);
        }




        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.Books.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        }


        //新增 //注意唯一性
        public async Task<OperationResult> CreateAsync(BookCreateVm vm, CancellationToken ct)  
        {
            string isbn = vm.Isbn.Trim();
            var result = new OperationResult();

            if (await _db.Books.AnyAsync(x => x.Isbn == isbn, ct)) 
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(vm.Isbn),
                        Message = "ISBN 已存在"
                    }
                );
            }


            string? fileName = null;
            try
            {
                if (vm.NewCover is not null)
                {
                    fileName = await _bCoverStorage.SaveImgAsync(vm.NewCover, ct);
                }
            }
            catch (InvalidOperationException ex)
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(vm.NewCover),
                        Message = ex.Message
                    }
                );
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var book = new Book
            {
                Isbn = isbn,
                Title = vm.Title.Trim(),
                Author = string.IsNullOrWhiteSpace(vm.Author) ? null : vm.Author.Trim(),
                BCoverFileName = fileName,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.Books.Add(book); 
            await _db.SaveChangesAsync(ct); 
            return result; 
        }


        //更新 //注意唯一性
        public async Task<OperationResult> UpdateAsync(int id, BookEditVm vm, CancellationToken ct)
        {

            var book = await _db.Books.SingleOrDefaultAsync(x => x.Id == id, ct); 
            var result = new OperationResult();
            if (book is null)
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = string.Empty,
                        Message = "找不到書籍"
                    }
                );
                return result;
            }

            string isbn = vm.Isbn.Trim();
            if (await _db.Books.AnyAsync(x => x.Isbn == isbn && x.Id != id, ct)) //不能改到別本書的 ISBN，檢查資料庫裡有沒有這個 ISBN 的書，且 Id 不同於目前要修改的書。
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(vm.Isbn),
                        Message = "ISBN 已存在"
                    }
                );
            }

            //編輯封面
            string? OriginalCover = book.BCoverFileName;
            try
            {
                if (vm.NewCover is not null)
                {
                    book.BCoverFileName = await _bCoverStorage.SaveImgAsync(vm.NewCover, ct);
                }
            }
            catch (InvalidOperationException ex)
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(vm.NewCover),
                        Message = ex.Message
                    }
                );
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            book.Isbn = isbn;
            book.Title = vm.Title.Trim();
            book.Author = string.IsNullOrWhiteSpace(vm.Author) ? null : vm.Author.Trim();
            await _db.SaveChangesAsync(ct);


            //刪除舊封面//如果舊封面檔名不為 null，且跟新的封面檔名不同，才刪除舊封面檔案。
            if (OriginalCover != book.BCoverFileName && OriginalCover != null && book.BCoverFileName != null)
            {
                _bCoverStorage.DeleteImg(OriginalCover);
            }
            
            return result;
        }


        //刪除
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var book = await _db.Books.SingleOrDefaultAsync(x => x.Id == id, ct);
            if (book == null)
            {
                return false;
            }
            _db.Books.Remove(book); 
            await _db.SaveChangesAsync(ct);

            if ( !string.IsNullOrWhiteSpace(book.BCoverFileName))
            {
                _bCoverStorage.DeleteImg(book.BCoverFileName);
            }

            return true;
        }


    }
}
