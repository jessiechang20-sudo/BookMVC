using Microsoft.AspNetCore.Hosting;
using System.Runtime.InteropServices;


namespace BookMvc.Services
{
    public sealed class BCoverStorage : IBCoverStorage
    {
        private readonly IWebHostEnvironment _env; //Web 主機的環境資訊，可以用來取得 Web 根目錄的路徑。
        public BCoverStorage(IWebHostEnvironment env) 
        {
            _env = env;
        }
        private static readonly HashSet<string> _allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)  //比較副檔名字串時忽略大小寫
        {
        ".jpg", ".jpeg", ".png"
        };

        private readonly long _maxFileSize = 5 * 1024 * 1024; //5MB



        //儲存書籍封面圖片，回傳圖片檔名
        public async Task<string> SaveImgAsync(IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
            {
                throw new InvalidOperationException("檔案是空的");
            }

            if (file.Length > _maxFileSize)
            {
                throw new InvalidOperationException("檔案大小超過限制");
            }

            string ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(file.FileName) || !_allowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException("檔案錯誤，只允許jpg / jpeg / png");
            }

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                throw new InvalidOperationException("檔案類型錯誤");
            }

            var uploadDir = Path.Combine(_env.WebRootPath, "upload");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var guid = Guid.NewGuid().ToString("N"); //檔名用GUID(不帶連字號)
            string fileName = $"{guid}{ext.ToLowerInvariant()}"; //並將副檔名轉換為小寫，以確保一致性的結果。
            var fullPath = Path.Combine(_env.WebRootPath, "upload", fileName); 
            await using var stream = File.Create(fullPath);  
            await file.CopyToAsync(stream, ct); 
            return fileName; 

        }



        //刪除書籍封面圖片
        public void DeleteImg(string fileName)
        {

            var fullPath = Path.Combine(_env.WebRootPath, "upload", fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath); //刪除指定路徑的檔案
            }
        }



    }
}
