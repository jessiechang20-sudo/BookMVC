namespace BookMvc.Services.Resaults
{
    public sealed class OperationResult
    {
        public bool Ok { get; set; } = true;//代表執行結果是否成功，true 代表成功，false 代表失敗。
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>(); //多筆驗證錯誤訊息
    }
}
