using System.ComponentModel.DataAnnotations;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Claims;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Services;
using learn.it.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginRequest)
        {
            User user;

            try
            {
                user = await _userService.GetUserByIdOrUsername(loginRequest.Username);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            if (!_userService.VerifyPassword(user, loginRequest.Password))
            {
                return Unauthorized();
            }
            var token = _userService.GenerateJwtToken(user);
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
            catch (EmailExistsException)
            {
                return Conflict("Email already exists");
            }
            catch (UsernameExistsException)
            {
                return Conflict("Username already exists");
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto updateRequest, [FromRoute] int userId)
        {
            var validationContext = new ValidationContext(updateRequest);
            var validation = updateRequest.Validate(validationContext);
            if (validation.Any())
            { 
                return BadRequest(validation);
            }

            try
            {
                await _userService.GetUserByIdOrUsername(userId.ToString());
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            var updatedUser = await _userService.UpdateUser(userId, updateRequest);
            return Ok(updatedUser.ToSelfUserResponseDto());
        }

        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetAll()
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
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
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
                return Ok(queriedUser.ToSelfUserResponseDto());
            }

            return Ok(queriedUser.ToAnonymousUserResponseDto());
        }

    }
}
