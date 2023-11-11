using learn.it.Models;
using learn.it.Models.Dtos;

namespace learn.it.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User userData);
        Task DeleteUser(int userId);
        Task<User> GetUserByIdOrUsername(string userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> UpdateUser(int userId, UpdateUserDto userData);
        string GenerateJwtToken(User user);
        bool VerifyPassword(User user, string password);
    }
}
