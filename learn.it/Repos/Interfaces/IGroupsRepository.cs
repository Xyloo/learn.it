using learn.it.Models;
using learn.it.Models.Dtos.Request;

namespace learn.it.Repos.Interfaces
{
    public interface IGroupsRepository
    {
        // Group related methods
        Task<Group> CreateGroup(Group group);
        Task DeleteGroup(Group group);
        Task<GroupDto> GetGroupDtoById(int groupId);
        Task<Group?> GetGroupById(int groupId);
        Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId);
        Task<IEnumerable<BasicGroupDto>> GetAllGroups();
        Task<Group> UpdateGroup(Group group);

        // Group join requests related methods
        Task<GroupJoinRequest> CreateGroupJoinRequest(GroupJoinRequest groupJoinRequest);
        Task RemoveGroupJoinRequest(GroupJoinRequest groupJoinRequest);
        Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForGroup(int groupId);
        Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForUser(int userId);
        Task<GroupJoinRequest?> GetGroupJoinRequest(int userId, int groupId);
    }

}
