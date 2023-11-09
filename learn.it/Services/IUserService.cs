using learn.it.Models;

namespace learn.it.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User userData);
        Task DeleteUser(int userId);
        Task<User> GetUserByIdOrUsername(string userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> UpdateUser(User userData);
        string GenerateJwtToken(User user);
        bool VerifyPassword(User user, string password);
    }
}
