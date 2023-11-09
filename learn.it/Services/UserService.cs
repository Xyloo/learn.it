using learn.it.Models;
using learn.it.Repos;
using learn.it.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using learn.it.Exceptions;

namespace learn.it.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public UserService(IUserRepository userRepository, PasswordHasher<User> passwordHasher, JwtSettings jwtSettings, IPermissionsRepository permissionsRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings;
            _permissionsRepository = permissionsRepository;
        }

        public async Task<User> CreateUser(User userData)
        {
            var existingUser = await _userRepository.GetUserByEmail(userData.Email);
            if (existingUser is not null)
            {
                throw new EmailExistsException();
            }
            existingUser = await _userRepository.GetUserByUsername(userData.Username);
            if (existingUser is not null)
            {
                throw new UsernameExistsException();
            }

            var hashedPassword = _passwordHasher.HashPassword(userData, userData.Password);
            userData.Password = hashedPassword;
            userData.CreateTime = DateTime.UtcNow;
            userData.Permissions = await _permissionsRepository.GetPermissionByName("User") ?? throw new InvalidOperationException("No permission was found with name 'User'");
            return await _userRepository.CreateUser(userData);
        }

        public async Task DeleteUser(int userId)
        {
            _ = await _userRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId.ToString());
            await _userRepository.DeleteUser(userId);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var role = user.Permissions.Name;
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, role.Trim())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User> GetUserByIdOrUsername(string userId)
        {
            var idParseSuccessful = int.TryParse(userId, out var id);
            User? user;
            if (idParseSuccessful)
            {
                user = await _userRepository.GetUserById(id);
            }
            else
            {
                user = await _userRepository.GetUserByUsername(userId);
            }

            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }
            return user;
        }

        public async Task<User> UpdateUser(User userData)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(User user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
