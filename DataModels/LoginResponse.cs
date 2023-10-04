namespace TransactionManager.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;
        public DateTime ExpirationDate { get; set; }    
    }
}
