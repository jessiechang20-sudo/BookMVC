using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Numerics;
using BookMvc.Data;
using BookMvc.Models.Entities;
using BookMvc.Models.ViewModels;
using BookMvc.Services.Resaults;
using BookMvc.Models.Inputs;
using BookMvc.Models.Common;

using BookMvc.Services;
using AspNetCoreGeneratedDocument;

namespace BookMvc.Services
{
    public sealed class BookService : IBookService
    {
        private readonly BookDbContext _db;
        private readonly IBCoverStorage _bCoverStorage;
        private readonly ILogger<BookService> _logger;
        public BookService(BookDbContext db, IBCoverStorage bCoverStorage, ILogger<BookService> logger)
        {
            _db = db;
            _bCoverStorage = bCoverStorage;
            _logger = logger;
        }

        //查詢
        public async Task<(List<Book> books, int totalcount)> GetListAsync(QueryInput input, CancellationToken ct)
        {
            var query = _db.Books.AsNoTracking();


            // (1)Search
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                input.Keyword = input.Keyword.Trim();
                query = query.Where(x => x.Isbn.Contains(input.Keyword) || x.Title.Contains(input.Keyword) || (x.Author != null && x.Author.Contains(input.Keyword)));
            }


            // (2)Sort
            query = input.Sort switch
            {
                SortOption.id => query.OrderBy(x => x.Id),
                SortOption.id_desc => query.OrderByDescending(x => x.Id),

                SortOption.isbn => query.OrderBy(x => x.Isbn),
                SortOption.isbn_desc => query.OrderByDescending(x => x.Isbn),

                SortOption.title => query.OrderBy(x => x.Title),
                SortOption.title_desc => query.OrderByDescending(x => x.Title),

                _ => query.OrderByDescending(x => x.Id)
            };

            // (3)Page
            input.Page = input.Page < 1 ? 1 : input.Page;
            int totalCount = await query.CountAsync(ct);
            var books = await query.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToListAsync(ct);

            return (books, totalCount);
        }




        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.Books.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        }


        //新增 //注意唯一性
        public async Task<(OperationResult, Book?)> CreateAsync(CreateInput input, CancellationToken ct)
        {
            string isbn = input.Isbn.Trim();
            var result = new OperationResult();

            if (await _db.Books.AnyAsync(x => x.Isbn == isbn, ct))
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = "Isbn",
                        Message = "ISBN 已存在"
                    }
                );
            }


            string? fileName = null;
            try
            {
                if (input.Image is not null)
                {
                    fileName = await _bCoverStorage.SaveImgAsync(input.Image, ct);
                }
            }
            catch (InvalidOperationException ex)
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = "Image",
                        Message = ex.Message
                    }
                );
                _logger.LogError(ex, "[BOOK_CREATE_UPLOAD_FAIL] Isbn: {Isbn} , Title: {Title} , ImgName: {ImgName}", input.Isbn, input.Title, input.Image?.FileName);
            }

            if (result.Errors.Count > 0)
            {
                return (result, null);
            }

            var book = new Book
            {
                Isbn = isbn,
                Title = input.Title.Trim(),
                Author = string.IsNullOrWhiteSpace(input.Author) ? null : input.Author.Trim(),
                BCoverFileName = fileName,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.Books.Add(book);

            try
            {
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("[BOOK_CREATE_SUCCESS] Id: {Id}, Isbn: {Isbn} , Title: {Title}", book.Id, book.Isbn, book.Title);
                return (result, book);
            }
            catch (Exception ex)
            {
                result.Ok = false;
                _logger.LogError(ex, "[BOOK_CREATE_DBSAVE_FAIL] Isbn: {Isbn} , Title: {Title}", book.Isbn, book.Title);
                return (result, null);
            }

        }


        //更新 //注意唯一性
        public async Task<(OperationResult, Book?)> UpdateAsync(int id, EditInput input, CancellationToken ct)
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
                return (result, null);
            }

            string isbn = input.Isbn.Trim();
            if (await _db.Books.AnyAsync(x => x.Isbn == isbn && x.Id != id, ct)) //不能改到別本書的 ISBN，檢查資料庫裡有沒有這個 ISBN 的書，且 Id 不同於目前要修改的書。
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(input.Isbn),
                        Message = "ISBN 已存在"
                    }
                );
            }

            //編輯封面
            string? OriginalCover = book.BCoverFileName;
            try
            {
                if (input.Image is not null)
                {
                    book.BCoverFileName = await _bCoverStorage.SaveImgAsync(input.Image, ct);
                }
            }
            catch (InvalidOperationException ex)
            {
                result.Ok = false;
                result.Errors.Add(
                    new ValidationError
                    {
                        Field = nameof(input.Image),
                        Message = ex.Message
                    }
                );
                _logger.LogError(ex, "[BOOK_EDIT_UPLOAD_FAIL] Id: {Id} , Title: {Title} , ImgName: {ImgName}", id, input.Title, input.Image?.FileName);
            }

            if (result.Errors.Count > 0)
            {
                return (result, null);
            }

            book.Isbn = isbn;
            book.Title = input.Title.Trim();
            book.Author = string.IsNullOrWhiteSpace(input.Author) ? null : input.Author.Trim();
            book.CreatedAt = DateTime.UtcNow;

            try
            {
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("[BOOK_EDIT_SUCCESS] Id: {Id}, Isbn: {Isbn} , Title: {Title}", book.Id, book.Isbn, book.Title);

                //刪除舊封面//如果舊封面檔名不為 null，且跟新的封面檔名不同，才刪除舊封面檔案。
                if (OriginalCover != book.BCoverFileName && OriginalCover != null && book.BCoverFileName != null)
                {
                    _bCoverStorage.DeleteImg(OriginalCover);
                }

                return (result, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BOOK_EDIT_DBSAVE_FAIL] Id: {Id}, Isbn: {Isbn} , Title: {Title}", book.Id, book.Isbn, book.Title);
                return (result, null);
            }
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

            try
            {
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("[BOOK_DELETE_SUCCESS] Id: {Id}", id);
                if (!string.IsNullOrWhiteSpace(book.BCoverFileName))
                {
                    _bCoverStorage.DeleteImg(book.BCoverFileName);
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BOOK_DALETE_FAIL] , Id: {Id} , Title: {Title}", id, book.Title);
                return false;
            }


        }


    }
}
