using AgricultureBackEnd.Data;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AgricultureBackEnd.Repositories.Implement
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