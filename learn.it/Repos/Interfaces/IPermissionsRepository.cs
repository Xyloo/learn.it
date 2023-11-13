using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IPermissionsRepository
    {
        Task<Permission?> GetPermissionById(int permissionId);
        Task<IEnumerable<Permission>> GetAllPermissions();
        Task<Permission?> GetPermissionByName(string permissionName);

    }
}
