using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> RoleNameExistsAsync(string roleName);
    }
}
