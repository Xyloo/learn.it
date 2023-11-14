using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class GroupJoinRequestsRepository : IGroupJoinRequestsRepository
    {
        private readonly LearnitDbContext _context;

        public GroupJoinRequestsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<GroupJoinRequest> CreateGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            await _context.GroupJoinRequests.AddAsync(groupJoinRequest);
            await _context.SaveChangesAsync();
            return groupJoinRequest;
        }

        public async Task DeleteGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            _context.GroupJoinRequests.Remove(groupJoinRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForGroup(int groupId)
        {
            return await _context.GroupJoinRequests.Where(g => g.GroupId == groupId).ToListAsync();
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForUser(int userId)
        {
            return await _context.GroupJoinRequests.Where(g => g.UserId == userId).ToListAsync();
        }
    }
}
