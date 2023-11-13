using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class LoginsRepository : ILoginsRepository
    {
        private readonly LearnitDbContext _context;

        public LoginsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<Login> CreateLogin(Login login)
        {
            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();
            return login;
        }

        public async Task<Login?> GetLastSuccessfulLogin(User user)
        {
            return await _context.Logins.Where(l => l.User.UserId == user.UserId && l.IsSuccessful).OrderByDescending(l => l.Timestamp).FirstOrDefaultAsync();
        }

        public async Task<Login?> GetLastUnsuccessfulLogin(User user)
        {
            return await _context.Logins.Where(l => l.User.UserId == user.UserId && !l.IsSuccessful).OrderByDescending(l => l.Timestamp).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Login>> GetUserLogins(User user)
        {
            return await _context.Logins.Where(l => l.User.UserId == user.UserId).ToListAsync();
        }
    }
}
