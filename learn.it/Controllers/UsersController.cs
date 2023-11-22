using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos;
using learn.it.Models.Dtos.Request;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpdateUserDto = learn.it.Models.Dtos.Request.UpdateUserDto;

namespace learn.it.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ILoginsService _loginsService;
        private readonly IGroupsService _groupsService;
        public UsersController(IUsersService usersService, ILoginsService loginsService, IGroupsService groupsService)
        {
            _usersService = usersService;
            _loginsService = loginsService;
            _groupsService = groupsService;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginRequest)
        {
            User user = await _usersService.GetUserByIdOrUsername(loginRequest.Username);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            if (!_usersService.VerifyPassword(user, loginRequest.Password))
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

            bool hasLoggedInToday = lastLogin.HasValue && lastLogin.Value.Date == DateTime.UtcNow.Date;
            bool hasLoggedInYesterday = lastLogin.HasValue && lastLogin.Value.Date == DateTime.UtcNow.Date.AddDays(-1);

            if (!hasLoggedInToday)
            {
                user.UserStats.ConsecutiveLoginDays = hasLoggedInYesterday
                    ? user.UserStats.ConsecutiveLoginDays + 1
                    : 1;
            }
            user.UserStats.TotalLoginDays += !hasLoggedInToday ? 1 : 0;
            user.LastLogin = DateTime.UtcNow;

            await _usersService.UpdateUser(user);

            var token = _usersService.GenerateJwtToken(user);
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
                await _usersService.CreateUser(user);
            }
            catch (EmailExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UsernameExistsException ex)
            {
                return Conflict(ex.Message);
            }

            return CreatedAtAction(nameof(GetUserById), new { userId = user.UserId }, user.ToSelfUserResponseDto());
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

            User queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var updatedUser = await _usersService.UpdateUser(userId, updateRequest);
                return Ok(updatedUser.ToSelfUserResponseDto());
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usersService.GetAllUsers();
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
            User queriedUser  = await _usersService.GetUserByIdOrUsername(userId);

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                return Ok(queriedUser.ToSelfUserResponseDto());
            }

            return Ok(queriedUser.ToAnonymousUserResponseDto());
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {

            User queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                await _usersService.DeleteUser(userId);
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
                    return BadRequest("The provided file is too large (max 10 MB).");
            }

            if (ControllerUtils.IsImage(avatar) is false)
            { 
                return BadRequest("The provided file is not an image.");
            }

            User queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var updatedUser = await _usersService.UpdateUserAvatar(queriedUser, avatar);
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
                queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                await _usersService.DeleteUserAvatar(queriedUser);
                return Ok("Avatar deleted successfully.");
            }

            return Unauthorized();
        }

        [HttpGet("{userId}/join-requests")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetUserJoinRequests([FromRoute] int userId)
        {
            User queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var joinRequests = await _groupsService.GetAllGroupJoinRequestsForUser(userId);
                return Ok(joinRequests);
            }

            return Unauthorized();
        }
    }
}
