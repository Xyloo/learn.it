using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Azure.Core.Pipeline;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupsService _groupsService;
        private readonly IUsersService _usersService;

        public GroupController(IGroupsService groupsService, IUsersService usersService)
        {
            _groupsService = groupsService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _groupsService.GetAllGroups();
            return Ok(groups);
        }

        [HttpGet("{groupId:int}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetGroupDetails([FromRoute] int groupId)
        {
            var group = await _groupsService.GetGroupDtoById(groupId);
            return Ok(group);
        }

        [HttpGet("find/{groupName}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> FindGroup([FromRoute] string groupName)
        {
            var groups = await _groupsService.FindGroups(groupName);
            return Ok(groups);
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateOrUpdateGroupDto groupDto)
        {
            var validationContext = new ValidationContext(groupDto);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(groupDto, validationContext, validationResults, true);
            if (!isValid)
            {
                return BadRequest(validationResults);
            }

            //this should never be null since [Authorize] is used
            var creator = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var group = new Group()
            {
                Name = groupDto.Name,
                Creator = creator
            };
            group.Users.Add(creator);
            group = await _groupsService.CreateGroup(group);
            return CreatedAtAction(nameof(GetGroupDetails), new { groupId = group.GroupId }, new GroupDto(group));
        }

        [HttpGet("{groupId}/join")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateJoinRequest([FromRoute] int groupId)
        {
            var userId = ControllerUtils.GetUserIdFromClaims(User);
            await _groupsService.CreateGroupJoinRequest(groupId, userId, userId);
            return Ok();
        }

        [HttpGet("{groupId}/invite/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateInvitation([FromRoute] int groupId, [FromRoute] int userId)
        {
            var creatorId = ControllerUtils.GetUserIdFromClaims(User);
            await _groupsService.CreateGroupJoinRequest(groupId, userId, creatorId);
            return Ok();
        }

        [HttpGet("{groupId}/join/{userId}/accept")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AcceptJoinRequest([FromRoute] int groupId, [FromRoute] int userId)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                await _groupsService.AcceptGroupJoinRequest(groupId, userId);
                return Ok();
            }
            return Forbid();
        }

        [HttpGet("{groupId}/join/{userId}/decline")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeclineJoinRequest([FromRoute] int groupId, [FromRoute] int userId)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                await _groupsService.RemoveGroupJoinRequest(groupId, userId);
                return Ok();
            }
            return Forbid();
        }

        [HttpGet("{groupId}/invite/accept")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AcceptInvitation([FromRoute] int groupId)
        {
            var userId = ControllerUtils.GetUserIdFromClaims(User);
            await _groupsService.AcceptGroupJoinRequest(groupId, userId);
            return Ok();
        }

        [HttpGet("{groupId}/invite/decline")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeclineInvitation([FromRoute] int groupId)
        {
            var userId = ControllerUtils.GetUserIdFromClaims(User);
            await _groupsService.RemoveGroupJoinRequest(groupId, userId);
            return Ok();
        }

        [HttpGet("{groupId}/remove/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> RemoveUserFromGroup([FromRoute] int groupId, [FromRoute] int userId)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                await _groupsService.RemoveUserFromGroup(userId, groupId);
                return Ok();
            } 
            return Forbid();
        }

        [HttpGet("{groupId}/leave")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> LeaveGroup([FromRoute] int groupId)
        {
            var userId = ControllerUtils.GetUserIdFromClaims(User);
            await _groupsService.RemoveUserFromGroup(userId, groupId);
            return Ok();
        }

        [HttpGet("{groupId}/add/{userId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> AddUserToGroup([FromRoute] int groupId, [FromRoute] int userId)
        {
            await _groupsService.AddUserToGroup(userId, groupId);
            return Ok();
        }

        [HttpPut("{groupId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateGroup([FromRoute] int groupId, [FromBody] CreateOrUpdateGroupDto groupDto)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                group = await _groupsService.UpdateGroup(groupDto, groupId);
                return Ok(group);
            }
            return Forbid();
        }

        [HttpDelete("{groupId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int groupId)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                await _groupsService.RemoveGroup(groupId);
                return Ok();
            }
            return Forbid();
        }

        [HttpGet("{groupId}/join-requests")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetGroupJoinRequests([FromRoute] int groupId)
        {
            var group = await _groupsService.GetGroupById(groupId);
            if (IsCreatorOrAdmin(group))
            {
                var joinRequests = await _groupsService.GetAllGroupJoinRequestsForGroup(groupId);
                return Ok(joinRequests);
            }
            return Forbid();
        }

        private bool IsCreatorOrAdmin(Group group)
        {
            var userId = ControllerUtils.GetUserIdFromClaims(User);
            return group.Creator.UserId == userId || User.HasClaim(ClaimTypes.Role, "Admin");
        }
    }
}
