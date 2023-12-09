using System.ComponentModel.DataAnnotations;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
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
        private readonly IStudySetsService _studySetsService;
        public UsersController(IUsersService usersService, ILoginsService loginsService, IGroupsService groupsService, IStudySetsService studySetsService)
        {
            _usersService = usersService;
            _loginsService = loginsService;
            _groupsService = groupsService;
            _studySetsService = studySetsService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginRequest)
        {
            var user = await _usersService.GetUserByIdOrUsername(loginRequest.Username);
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
                throw new InvalidInputDataException("Username and/or password are incorrect.");
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
            return Ok(new {
                token = token,
                userId = user.UserId
            });
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

            await _usersService.CreateUser(user);

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
                throw new InvalidInputDataException(validation.ToString());
            }

            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var updatedUser = await _usersService.UpdateUser(userId, updateRequest);
                return Ok(updatedUser.ToSelfUserResponseDto());
            }

            return Forbid();
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
            var queriedUser  = await _usersService.GetUserByIdOrUsername(userId);

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

            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                await _usersService.DeleteUser(userId);
                return Ok();
            }

            return Forbid();
        }

        [HttpPost("avatar/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateUserAvatar([FromRoute] int userId, IFormFile avatar)
        {
            ControllerUtils.CheckIfValidImage(avatar);

            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var updatedUser = await _usersService.UpdateUserAvatar(queriedUser, avatar);
                return Ok("Avatar updated successfully.");
            }

            return Forbid();
        }

        [HttpDelete("avatar/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteUserAvatar([FromRoute] int userId)
        {
            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                await _usersService.DeleteUserAvatar(queriedUser);
                return Ok("Avatar deleted successfully.");
            }

            return Forbid();
        }

        [HttpGet("{userId}/join-requests")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetUserJoinRequests([FromRoute] int userId)
        {
            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var joinRequests = await _groupsService.GetAllGroupJoinRequestsForUser(userId);
                return Ok(joinRequests);
            }

            return Forbid();
        }

        [HttpGet("{userId}/groups")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetUserGroups([FromRoute] int userId)
        {
            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if (ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var groups = queriedUser.Groups.Select(g => g.ToGroupDto()).ToList();
                return Ok(groups);
            }

            return Forbid();
        }

        [HttpGet("{userId}/studysets")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetUserStudyStes([FromRoute] int userId)
        {
            var queriedUser = await _usersService.GetUserByIdOrUsername(userId.ToString());

            if(ControllerUtils.IsUserAdminOrSelf(queriedUser, User))
            {
                var studySets = await _studySetsService.GetAllStudySetsByCreator(userId);
                return Ok(studySets);
            }
            return Forbid();
        }

    }
}
