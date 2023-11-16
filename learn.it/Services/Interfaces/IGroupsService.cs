using learn.it.Models;
using learn.it.Models.Dtos.Request;

namespace learn.it.Services.Interfaces
{
    public interface IGroupsService
    {
        Task<Group> CreateGroup(Group group);
        Task RemoveGroup(int groupId);
        Task<IEnumerable<BasicGroupDto>> GetAllGroups();
        Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId);
        Task<Group> GetGroupById(int groupId);
        Task<GroupDto> GetGroupDtoById(int groupId);
        Task<Group> UpdateGroup(Group group);
        Task<Group> UpdateGroup(CreateOrUpdateGroupDto groupDto, int groupId);
        Task<IEnumerable<BasicGroupDto>> FindGroups(string name);
        Task<Group> AddUserToGroup(int userId, int groupId);
        Task<Group?> RemoveUserFromGroup(int userId, int groupId);

        Task<GroupJoinRequest> CreateGroupJoinRequest(int groupId, int userId, int creatorId);
        Task RemoveGroupJoinRequest(int groupId, int userId);
        Task AcceptGroupJoinRequest(int groupId, int userId);
        Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForGroup(int groupId);
        Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForUser(int userId);
    }
}
