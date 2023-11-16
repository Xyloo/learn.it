using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly LearnitDbContext _context;

        public PermissionsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetPermissionById(int permissionId)
        {
            return await _context.Permissions.FindAsync(permissionId);
        }

        public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission?> GetPermissionByName(string permissionName)
        {
            return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == permissionName);
        }
    }
}
