namespace WebApplication1.Services.Resaults
{
    public sealed class ValidationError
    {
        public string? Field { get; set; } 
        public string Message { get; set; } = "";
    }
}
