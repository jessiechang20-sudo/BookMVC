using BookMvc.Models.Entities;
using BookMvc.Models.Inputs;
using BookMvc.Models.ViewModels;
using BookMvc.Services.Resaults; 

namespace BookMvc.Services
{
    public interface IBookService
    {
        //查詢書籍清單
        Task<(List<Book> books , int totalcount)> GetListAsync(QueryInput input , CancellationToken ct);

        //查詢單一書籍
        Task<Book?> GetByIdAsync(int id, CancellationToken ct);


        //新增書籍
        Task<(OperationResult, Book?)> CreateAsync(CreateInput input, CancellationToken ct);  //表單驗證失敗其實很常見，不想用 Exception 流程；service 只回「成功/失敗」，畫面要顯示什麼再另外查或用 ViewModel

        //更新書籍資訊
        Task<(OperationResult, Book?)> UpdateAsync(int id , EditInput input , CancellationToken ct); //BookEditVm 本身就會有 Id，因此方法不需要再多收一個 id

        //刪除書籍
        Task<bool> DeleteAsync(int id, CancellationToken ct); //bool：代表刪除成功與否，true 代表成功刪除，false 代表找不到對應的書籍或刪除失敗。


    }
}
