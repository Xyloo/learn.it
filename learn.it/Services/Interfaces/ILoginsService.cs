using learn.it.Models;

namespace learn.it.Services.Interfaces
{
    public interface ILoginsService
    {
        Task<Login> CreateLogin(Login login);
        Task<Login?> GetLastSuccessfulLogin(User user);
        Task<Login?> GetLastUnsuccessfulLogin(User user);
        Task<IEnumerable<Login>> GetUserLogins(User user);
    }
}
