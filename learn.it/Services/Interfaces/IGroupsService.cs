using learn.it.Models;
using learn.it.Models.Dtos.Request;

namespace learn.it.Services.Interfaces
{
    public interface IGroupsService
    {
        Task<Group> CreateGroup(Group group);
        Task DeleteGroup(Group group);
        Task<IEnumerable<BasicGroupDto>> GetAllGroups();
        Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId);
        Task<GroupDto> GetGroupById(int groupId);
        Task<Group> UpdateGroup(Group group);
        Task<Group> UpdateGroup(CreateOrUpdateGroupDto groupDto, int groupId);

        Task<GroupJoinRequest> CreateGroupJoinRequest(int groupId, int userId, int creatorId);
        Task RemoveGroupJoinRequest(int groupId, int userId);
        Task AcceptGroupJoinRequest(int groupId, int userId);
        Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForGroup(int groupId);
        Task<IEnumerable<GroupJoinRequestDto>> GetAllGroupJoinRequestsForUser(int userId);
    }
}
