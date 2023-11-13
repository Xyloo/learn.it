using learn.it.Models;
using learn.it.Repos.Interfaces;

namespace learn.it.Services
{
    public class LoginsService : ILoginsService
    {
        private readonly ILoginsRepository _loginsRepository;

        public LoginsService(ILoginsRepository loginsRepository)
        {
            _loginsRepository = loginsRepository;
        }

        public async Task<Login> CreateLogin(Login login)
        {
            return await _loginsRepository.CreateLogin(login);
        }

        public async Task<Login?> GetLastSuccessfulLogin(User user)
        {
            return await _loginsRepository.GetLastSuccessfulLogin(user);
        }

        public async Task<Login?> GetLastUnsuccessfulLogin(User user)
        {
            return await _loginsRepository.GetLastUnsuccessfulLogin(user);
        }

        public async Task<IEnumerable<Login>> GetUserLogins(User user)
        {
            return await _loginsRepository.GetUserLogins(user);
        }
    }
}
