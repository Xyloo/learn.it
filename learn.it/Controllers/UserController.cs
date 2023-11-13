using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos;
using learn.it.Models.Dtos.Request;
using learn.it.Services;
using learn.it.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoginsService _loginsService;
        public UserController(IUserService userService, ILoginsService loginsService)
        {
            _userService = userService;
            _loginsService = loginsService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginRequest)
        {
            User user = await _userService.GetUserByIdOrUsername(loginRequest.Username);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            if (!_userService.VerifyPassword(user, loginRequest.Password))
            {
                var login = new Login()
                {
                    User = user,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    IsSuccessful = false,
                    Timestamp = DateTime.UtcNow
                };
                await _loginsService.CreateLogin(login);
                return Unauthorized("Username and/or password are incorrect.");
            }

            var lastLogin = user.LastLogin;

            if (lastLogin == null || lastLogin.Value.Date != DateTime.UtcNow.Date && lastLogin.Value.Date != DateTime.UtcNow.Date.AddDays(-1))
            {
                user.LastLogin = DateTime.UtcNow;
                user.UserStats.ConsecutiveLoginDays = 1;
            }
            else
            {
                user.UserStats.ConsecutiveLoginDays++;
                user.LastLogin = DateTime.UtcNow;
            }


            await _userService.UpdateUser(user);

            var token = _userService.GenerateJwtToken(user);
            var successfulLogin = new Login()
            {
                User = user,
                IpAddress = ip,
                UserAgent = userAgent,
                IsSuccessful = true,
                Timestamp = DateTime.UtcNow
            };
            await _loginsService.CreateLogin(successfulLogin);
            return Ok(new { token });
        }

        [HttpGet("logout")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Logout()
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            InvalidJwtTokens.AddToken(token!);
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerRequest)
        {
            var user = new User
            {
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };

            try
            {
                await _userService.CreateUser(user);
            }
            catch (EmailExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UsernameExistsException ex)
            {
                return Conflict(ex.Message);
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateRequest, [FromRoute] int userId)
        {
            var validationContext = new ValidationContext(updateRequest);
            var validation = updateRequest.Validate(validationContext);
            if (validation.Any())
            { 
                return BadRequest(validation);
            }

            User queriedUser = await _userService.GetUserByIdOrUsername(userId.ToString());

            if (IsUserAdminOrSelf(queriedUser))
            {
                var updatedUser = await _userService.UpdateUser(userId, updateRequest);
                return Ok(updatedUser.ToSelfUserResponseDto());
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            foreach (var user in users)
            {
                user.Password = null!;
            }
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetUserById([FromRoute] string userId)
        {
            User queriedUser  = await _userService.GetUserByIdOrUsername(userId);

            if (IsUserAdminOrSelf(queriedUser))
            {
                return Ok(queriedUser.ToSelfUserResponseDto());
            }

            return Ok(queriedUser.ToAnonymousUserResponseDto());
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {

            User queriedUser = await _userService.GetUserByIdOrUsername(userId.ToString());

            if (IsUserAdminOrSelf(queriedUser))
            {
                await _userService.DeleteUser(userId);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("avatar/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateUserAvatar([FromRoute] int userId, IFormFile avatar)
        {
            switch (avatar.Length)
            {
                case 0:
                    return BadRequest("No avatar file was provided.");
                case > 10 * 1024 * 1024:
                    return BadRequest("The provided file is too large.");
            }

            if (IsImage(avatar) is false)
            { 
                return BadRequest("The provided file is not an image.");
            }

            User queriedUser = await _userService.GetUserByIdOrUsername(userId.ToString());

            if (IsUserAdminOrSelf(queriedUser))
            {
                var updatedUser = await _userService.UpdateUserAvatar(queriedUser, avatar);
                return Ok("Avatar updated successfully.");
            }

            return Unauthorized();
        }

        [HttpDelete("avatar/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteUserAvatar([FromRoute] int userId)
        {
            User queriedUser;
            try
            {
                queriedUser = await _userService.GetUserByIdOrUsername(userId.ToString());
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            if (User.FindFirst(ClaimTypes.Role)?.Value == "Admin" || User.Identity?.Name == queriedUser.Username)
            {
                await _userService.DeleteUserAvatar(queriedUser);
                return Ok("Avatar deleted successfully.");
            }

            return Unauthorized();
        }

        private bool IsImage(IFormFile file)
        {
            // Check the file content type
            if (file.ContentType.ToLower().StartsWith("image/"))
            {
                return true;
            }

            // Alternatively, check the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(extension);
        }

        private bool IsUserAdminOrSelf(User user)
        {
            return User.FindFirst(ClaimTypes.Role)?.Value == "Admin" || User.Identity?.Name == user.Username;
        }

    }
}
