using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IGroupJoinRequestsRepository
    {
        Task<GroupJoinRequest> CreateGroupJoinRequest(GroupJoinRequest groupJoinRequest);
        Task DeleteGroupJoinRequest(GroupJoinRequest groupJoinRequest);
        Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForGroup(int groupId);
        Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForUser(int userId);
    }
}
