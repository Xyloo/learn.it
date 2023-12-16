using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly LearnitDbContext _context;

        public GroupsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<Group> CreateGroup(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task DeleteGroup(Group group)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BasicGroupDto>> GetAllGroups()
        {
            return await _context.Groups.Include(g => g.Creator).Include(g => g.Users).Select(g => g.ToBasicGroupDto()).ToListAsync();
        }

        public async Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId)
        {
            return await _context.Groups.Where(g => g.Creator.UserId == ownerId)
                .Include(g => g.Creator)
                .Include(g => g.Users)
                .Include(g => g.StudySets)
                .Select(g => g.ToGroupDto()).ToListAsync();
        }

        public async Task<GroupDto> GetGroupDtoById(int groupId)
        {
            var group = await _context.Groups.Include(g => g.Creator)
                .Include(g => g.Users)
                .Include(g => g.StudySets)
                .FirstOrDefaultAsync(g => g.GroupId == groupId) ?? throw new GroupNotFoundException(groupId.ToString());
            return new GroupDto(group);
        }

        public async Task<Group?> GetGroupById(int groupId)
        {
            return await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Users)
                .Include(g => g.GroupJoinRequests)
                .Include(g => g.StudySets)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<Group> UpdateGroup(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<GroupJoinRequest> CreateGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            await _context.GroupJoinRequests.AddAsync(groupJoinRequest);
            await _context.SaveChangesAsync();
            return groupJoinRequest;
        }

        public async Task RemoveGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            _context.GroupJoinRequests.Remove(groupJoinRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForGroup(int groupId)
        {
            return await _context.GroupJoinRequests.Where(request => request.GroupId == groupId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .ThenInclude(g => g.Creator)
                .Include(g => g.Group.Users)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForUser(int userId)
        {
            return await _context.GroupJoinRequests.Where(request => request.UserId == userId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .ThenInclude(g => g.Creator)
                .Include(g => g.Group.Users)
                .ToListAsync();
        } 

        public async Task<GroupJoinRequest?> GetGroupJoinRequest(int userId, int groupId)
        {
            return await _context.GroupJoinRequests.FindAsync(userId, groupId);
        }

        public async Task<IEnumerable<BasicGroupDto>> FindGroups(string name)
        {
            return await _context.Groups.Where(g => g.Name.Contains(name))
                .Include(g => g.Creator)
                .Include(g => g.Users)
                .Select(g => g.ToBasicGroupDto()).ToListAsync();
        }
    }
}