using learn.it.Models;

namespace learn.it.Repos
{
    public interface ILoginsRepository
    {
        Task<Login> CreateLogin(Login login);
        Task<IEnumerable<Login>> GetUserLogins(User user);
        Task<Login?> GetLastSuccessfulLogin(User user);
        Task<Login?> GetLastUnsuccessfulLogin(User user);
    }
}
