using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
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

        public async Task<IEnumerable<GroupDto>> GetAllGroups()
        {
            return await _context.Groups.Select(g => new GroupDto(g)).ToListAsync();
        }

        public async Task<IEnumerable<GroupDto>> GetAllOwnedGroups(int ownerId)
        {
            return await _context.Groups.Where(g => g.Owner.UserId == ownerId).Select(g => new GroupDto(g)).ToListAsync();
        }

        public async Task<GroupDto?> GetGroupById(int groupId)
        {
            var group = await _context.Groups.FindAsync(groupId) ?? throw new GroupNotFoundException(groupId.ToString());
            return new GroupDto(group);
        }

        public async Task<Group> UpdateGroup(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }
    }
}
