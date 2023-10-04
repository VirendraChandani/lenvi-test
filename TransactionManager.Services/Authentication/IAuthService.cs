namespace TransactionManager.Services.Authentication
{
    public interface IAuthService
    {
        bool Authenticate(string userName, string password);
        string GenerateJwtToken(string userName);
    }
}
