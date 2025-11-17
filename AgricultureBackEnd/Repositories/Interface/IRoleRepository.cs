using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.Repositories.Interface
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> RoleNameExistsAsync(string roleName);
    }
}