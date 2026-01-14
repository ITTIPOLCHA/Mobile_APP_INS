public interface IAuthService
{
    Task<(bool IsSuccess, string Message)> LoginAsync(string username, string password);
    Task LogoutAsync();
}