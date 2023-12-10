using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    /// <summary>
    /// Repository layer for the User model.
    /// It is assumed all methods are called from within a try-catch block.
    /// It is also assumed that the provided data is valid.
    /// </summary>
    public class UsersRepository : IUsersRepository
    {
        private readonly LearnitDbContext _context;

        public UsersRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.Include(u => u.Permissions).ToListAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Permissions)
                .Include(u => u.UserStats)
                .Include(u => u.UserPreferences)
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users
                .Include(u => u.Permissions)
                .Include(u => u.UserStats)
                .Include(u => u.UserPreferences)
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users
                .Include(u => u.Permissions)
                .Include(u => u.UserStats)
                .Include(u => u.UserPreferences)
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
