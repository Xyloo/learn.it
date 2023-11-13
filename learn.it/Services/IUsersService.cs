using learn.it.Models;
using learn.it.Models.Dtos;

namespace learn.it.Services
{
    public interface IUsersService
    {
        Task<User> CreateUser(User userData);
        Task DeleteUser(int userId);
        Task<User> GetUserByIdOrUsername(string userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> UpdateUser(int userId, UpdateUserDto userData);
        Task<User> UpdateUser(User user);
        string GenerateJwtToken(User user);
        bool VerifyPassword(User user, string password);
        Task<User> UpdateUserAvatar(User user, IFormFile avatar);
        Task DeleteUserAvatar(User user);
    }
}
