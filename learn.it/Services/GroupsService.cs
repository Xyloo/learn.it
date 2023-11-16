using System.ComponentModel.DataAnnotations;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUsersRepository _usersRepository;

        public GroupsService(IGroupsRepository groupsRepository, IUsersRepository usersRepository)
        {
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<Group> CreateGroup(Group group)
        {
            if (await _usersRepository.GetUserById(group.Creator.UserId) == null)
            {
                throw new ArgumentException("Creator does not exist");
            }

            return await _groupsRepository.CreateGroup(group);
        }

        public async Task DeleteGroup(Group group)
        {
            await GetGroupByIdOrThrow(group.GroupId);
            await _groupsRepository.DeleteGroup(group);
        }

        public async Task<IEnumerable<BasicGroupDto>> GetAllGroups()
        {
            return await _groupsRepository.GetAllGroups();
        }

        public async Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId)
        {
            return await _groupsRepository.GetAllOwnedGroups(ownerId);
        }

        public async Task<GroupDto> GetGroupById(int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            return group.ToGroupDto();
        }

        public async Task<Group> UpdateGroup(Group group)
        {
            var validationContext = new ValidationContext(group);
            var nameValidation = new CreateOrUpdateGroupDto(group.Name).Validate(validationContext);
            if (nameValidation.Any())
            {
                throw new ArgumentException(nameValidation.ToString());
            }
            return await _groupsRepository.UpdateGroup(group);
        }

        public async Task<Group> UpdateGroup(CreateOrUpdateGroupDto groupDto, int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            var validationContext = new ValidationContext(groupDto);
            var validationResults = groupDto.Validate(validationContext);
            if (validationResults.Any())
            {
                throw new ArgumentException(validationResults.ToString());
            }
            group.Name = groupDto.Name!;
            return await UpdateGroup(group);
        }

        public async Task<GroupJoinRequest> CreateGroupJoinRequest(int groupId, int userId, int creatorId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            await GetUserByIdOrThrow(userId);
            var creator = await GetUserByIdOrThrow(creatorId);

            if (group.Creator.UserId == userId || group.Users.Any(u => u.UserId == userId))
            {
                throw new ArgumentException("User is already a member of this group.");
            }

            if (await _groupsRepository.GetGroupJoinRequest(groupId, userId) != null)
            {
                throw new ArgumentException("User has already requested to join this group.");
            }

            var groupJoinRequest = new GroupJoinRequest
            {
                GroupId = groupId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Creator = creator
            };

            return await _groupsRepository.CreateGroupJoinRequest(groupJoinRequest);
        }

        public async Task RemoveGroupJoinRequest(int groupId, int userId)
        {
            await GetGroupByIdOrThrow(groupId);
            await GetUserByIdOrThrow(userId);

            var groupJoinRequest = await _groupsRepository.GetGroupJoinRequest(groupId, userId)
                                   ?? throw new GroupJoinRequestNotFoundException(userId.ToString(), groupId.ToString());

            await _groupsRepository.RemoveGroupJoinRequest(groupJoinRequest);
        }

        public async Task AcceptGroupJoinRequest(int groupId, int userId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            var user = await GetUserByIdOrThrow(userId);

            var groupJoinRequest = await _groupsRepository.GetGroupJoinRequest(groupId, userId)
                                   ?? throw new GroupJoinRequestNotFoundException(userId.ToString(), groupId.ToString());

            group.Users.Add(user);
            await _groupsRepository.UpdateGroup(group);
            await _groupsRepository.RemoveGroupJoinRequest(groupJoinRequest);
        }

        public async Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForGroup(int groupId)
        {
            var requests = await _groupsRepository.GetAllGroupJoinRequestsForGroup(groupId);
            return requests.Select(r => r.ToGroupJoinRequestDto());
        }

        public async Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForUser(int userId)
        {
            var requests = await _groupsRepository.GetAllGroupJoinRequestsForUser(userId);
            return requests.Select(r => r.ToGroupJoinRequestDto());
        }

        private async Task<Group> GetGroupByIdOrThrow(int groupId)
        {
            var group = await _groupsRepository.GetGroupById(groupId) ??
                        throw new GroupNotFoundException(groupId.ToString());
            return group;
        }

        private async Task<User> GetUserByIdOrThrow(int userId)
        {
            var user = await _usersRepository.GetUserById(userId) ??
                       throw new UserNotFoundException(userId.ToString());
            return user;
        }
    }
}
