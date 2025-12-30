using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await _context.Roles.AnyAsync(r => r.RoleName == roleName);
        }
    }
}