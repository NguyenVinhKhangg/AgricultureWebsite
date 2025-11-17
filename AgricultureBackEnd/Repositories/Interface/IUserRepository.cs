using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.Repositories.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetWithAddressesAsync(int userId);
        Task<IEnumerable<User>> GetByRoleAsync(int roleId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}