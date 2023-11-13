using learn.it.Models;
using learn.it.Models.Dtos.Request;

namespace learn.it.Repos.Interfaces
{
    public interface IGroupsRepository
    {
        Task<Group> CreateGroup(Group group);
        Task DeleteGroup(Group group);
        Task<GroupDto?> GetGroupById(int groupId);
        Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId);
        Task<IEnumerable<GroupDto>> GetAllGroups();
        Task<Group> UpdateGroup(Group group);
    }
}
