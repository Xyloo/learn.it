using System.ComponentModel.DataAnnotations;
using learn.it.Exceptions;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
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
                throw new UserNotFoundException(group.Creator.UserId.ToString());
            }

            return await _groupsRepository.CreateGroup(group);
        }

        public async Task RemoveGroup(int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            await _groupsRepository.DeleteGroup(group);
        }

        public async Task<IEnumerable<BasicGroupDto>> GetAllGroups()
        {
            return await _groupsRepository.GetAllGroups();
        }

        public async Task<bool> IsUserInGroup(int userId, int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            return group.Users.Any(u => u.UserId == userId);
        }

        public async Task<Group> GetGroupById(int groupId)
        {
            return await GetGroupByIdOrThrow(groupId);
        }

        public async Task<GroupDto> GetGroupDtoById(int groupId)
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
                throw new InvalidInputDataException(nameValidation.ToString());
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
                throw new InvalidInputDataException(validationResults.ToString());
            }
            group.Name = groupDto.Name!;
            return await UpdateGroup(group);
        }

        public async Task<IEnumerable<BasicGroupDto>> FindGroups(string name)
        {
            return await _groupsRepository.FindGroups(name);
        }

        public async Task<Group> AddUserToGroup(int userId, int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            var user = await GetUserByIdOrThrow(userId);

            if (group.Users.Any(u => u.UserId == user.UserId))
            {
                throw new InvalidInputDataException("Użytkownik należy już do tej grupy.");
            }

            group.Users.Add(user);
            return await _groupsRepository.UpdateGroup(group);
        }

        public async Task<Group?> RemoveUserFromGroup(int userId, int groupId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            var user = await GetUserByIdOrThrow(userId);

            if (group.Users.All(u => u.UserId != user.UserId))
            {
                throw new InvalidInputDataException("Użytkownik nie należy do podanej grupy.");
            }

            group.Users.Remove(user);
            if (group.Creator.UserId == userId)
            {
                await RemoveGroup(groupId);
                return default;
            }
            return await _groupsRepository.UpdateGroup(group);
        }

        public async Task<GroupJoinRequest> CreateGroupJoinRequest(int groupId, int userId, int creatorId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            await GetUserByIdOrThrow(userId);
            var creator = await GetUserByIdOrThrow(creatorId);

            if (group.Users.Any(u => u.UserId == userId))
            {
                throw new InvalidInputDataException("Użytkownik należy już do tej grupy.");
            }

            if (userId != creatorId && group.Creator.UserId != creatorId)
            {
                throw new InvalidInputDataException("Tylko twórca grupy może zapraszać innych użytkowników.");
            }

            if (await _groupsRepository.GetGroupJoinRequest(userId, groupId) != null)
            {
                throw new InvalidInputDataException("Użytkownik złożył już prośbę o dołączenie do tej grupy.");
            }

            var groupJoinRequest = new GroupJoinRequest
            {
                GroupId = groupId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Creator = creator,
                Group = group
            };

            return await _groupsRepository.CreateGroupJoinRequest(groupJoinRequest);
        }

        public async Task RemoveGroupJoinRequest(int groupId, int userId)
        {
            await GetGroupByIdOrThrow(groupId);
            await GetUserByIdOrThrow(userId);

            var groupJoinRequest = await _groupsRepository.GetGroupJoinRequest(userId, groupId)
                                   ?? throw new GroupJoinRequestNotFoundException(userId.ToString(), groupId.ToString());

            await _groupsRepository.RemoveGroupJoinRequest(groupJoinRequest);
        }

        public async Task AcceptGroupJoinRequest(int groupId, int userId)
        {
            var group = await GetGroupByIdOrThrow(groupId);
            var user = await GetUserByIdOrThrow(userId);

            var groupJoinRequest = await _groupsRepository.GetGroupJoinRequest(userId, groupId)
                                   ?? throw new GroupJoinRequestNotFoundException(userId.ToString(), groupId.ToString());
            if (groupJoinRequest.ExpiresAt < DateTime.UtcNow)
            {
                await _groupsRepository.RemoveGroupJoinRequest(groupJoinRequest);
                throw new InvalidInputDataException("Prośba o dołączenie wygasła.");
            }
            group.Users.Add(user);
            await _groupsRepository.UpdateGroup(group);
            await _groupsRepository.RemoveGroupJoinRequest(groupJoinRequest);
        }

        public async Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForGroup(int groupId)
        {
            await GetGroupByIdOrThrow(groupId);
            var requests = (await _groupsRepository.GetAllGroupJoinRequestsForGroup(groupId)).ToList();
            for (int i = requests.Count - 1; i >= 0; i--)
            {
                var r = requests[i];
                if (r.ExpiresAt < DateTime.UtcNow)
                {
                    await _groupsRepository.RemoveGroupJoinRequest(r);
                    requests.RemoveAt(i);
                }
            }
            return requests.Select(r => r.ToGroupJoinRequestDto());
        }

        public async Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForUser(int userId)
        {
            await GetUserByIdOrThrow(userId);
            var requests = (await _groupsRepository.GetAllGroupJoinRequestsForUser(userId)).ToList();
            for (int i = requests.Count - 1; i >= 0; i--)
            {
                var r = requests[i];
                if (r.ExpiresAt < DateTime.UtcNow)
                {
                    await _groupsRepository.RemoveGroupJoinRequest(r);
                    requests.RemoveAt(i);
                }
            }
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
