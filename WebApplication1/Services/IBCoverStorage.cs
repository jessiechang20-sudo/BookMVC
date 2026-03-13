namespace WebApplication1.Services
{
    public interface IBCoverStorage
    {
        //儲存書籍封面圖片，回傳圖片檔名
        Task<string> SaveImgAsync(IFormFile file, CancellationToken ct);


        //刪除書籍封面圖片
        void DeleteImg(string fileName);


    }
}
