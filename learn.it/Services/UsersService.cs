﻿using learn.it.Models;
using learn.it.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using learn.it.Exceptions;
using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models.Dtos.Request;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageHandler _imageHandler;
        private const string AvatarsDirectory = "Avatars";

        public UsersService(IUsersRepository usersRepository, IPermissionsRepository permissionsRepository, IWebHostEnvironment webHostEnvironment, IImageHandler imageHandler)
        {
            _usersRepository = usersRepository;
            _passwordHasher = new PasswordHasher<User>();
            _permissionsRepository = permissionsRepository;
            _webHostEnvironment = webHostEnvironment;
            _imageHandler = imageHandler;
        }

        public async Task<User> CreateUser(User userData)
        {
            var existingUser = await _usersRepository.GetUserByEmail(userData.Email);
            if (existingUser is not null)
            {
                throw new EmailExistsException();
            }
            existingUser = await _usersRepository.GetUserByUsername(userData.Username);
            if (existingUser is not null)
            {
                throw new UsernameExistsException();
            }

            var hashedPassword = _passwordHasher.HashPassword(userData, userData.Password);
            userData.Password = hashedPassword;
            userData.CreateTime = DateTime.UtcNow;
            userData.Permissions = await _permissionsRepository.GetPermissionByName("User") ?? throw new InvalidInputDataException("Nie odnaleziono poziomu uprawnień 'User'.");
            userData.UserStats = new UserStats {User = userData};
            userData.UserPreferences = new UserPreferences {User = userData};
            return await _usersRepository.CreateUser(userData);
        }

        public async Task DeleteUser(int userId)
        {
            _ = await _usersRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId.ToString());
            await _usersRepository.DeleteUser(userId);
        }

        public async Task DeleteUserAvatar(User user)
        {
            if (user.Avatar == null)
                return;

            var path = Path.Combine(_webHostEnvironment.WebRootPath, AvatarsDirectory, user.Avatar);
            if(File.Exists(path))
                File.Delete(path);

            user.Avatar = null;
            await _usersRepository.UpdateUser(user);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettings.Key);
            var role = user.Permissions.Name;
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, role.Trim()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = JwtSettings.Issuer,
                Audience = JwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _usersRepository.GetAllUsers();
        }

        public async Task<User> GetUserByIdOrUsername(string userId)
        {
            var idParseSuccessful = int.TryParse(userId, out var id);
            User? user;
            if (idParseSuccessful)
            {
                user = await _usersRepository.GetUserById(id);
            }
            else
            {
                user = await _usersRepository.GetUserByUsername(userId);
            }

            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }
            return user;
        }

        public async Task<User> UpdateUser(int userId, UpdateUserDto userData)
        {
            var user = await _usersRepository.GetUserById(userId) ?? throw new UserNotFoundException(userId.ToString());
            if (userData.Username is not null)
            {
                var existingUser = await _usersRepository.GetUserByUsername(userData.Username);
                if (existingUser is not null && existingUser.UserId != userId)
                {
                    throw new UsernameExistsException();
                }
                user.Username = userData.Username;
            }

            if (userData.Email is not null)
            {
                var existingUser = await _usersRepository.GetUserByEmail(userData.Email);
                if (existingUser is not null && existingUser.UserId != userId)
                {
                    throw new EmailExistsException();
                }
                user.Email = userData.Email;
            }

            if (userData.Password is not null)
            {
                var hashedPassword = _passwordHasher.HashPassword(user, userData.Password);
                user.Password = hashedPassword;
            }

            return await _usersRepository.UpdateUser(user);
        }

        /// <summary>
        /// This method should only be used with trusted data.
        /// It does not check anything.
        /// It is used to update LastLogin, UserStats and UserPreferences.
        /// </summary>
        /// <param name="user">User to be updated, with already new fields.</param>
        /// <returns>The same user.</returns>
        public async Task<User> UpdateUser(User user)
        {
            return await _usersRepository.UpdateUser(user);
        }

        public async Task<User> UpdateUserAvatar(User user, IFormFile avatar)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, AvatarsDirectory);
            var fileName = await _imageHandler.AddImage(avatar, path);
            user.Avatar = fileName;
            return await _usersRepository.UpdateUser(user);
        }

        public bool VerifyPassword(User user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return verificationResult == PasswordVerificationResult.Success;
        }

    }
}
