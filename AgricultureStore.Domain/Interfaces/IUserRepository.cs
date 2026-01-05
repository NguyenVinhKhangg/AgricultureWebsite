using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetWithAddressesAsync(int userId);
        Task<IEnumerable<User>> GetByRoleAsync(int roleId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        // Paginated methods
        Task<(IEnumerable<User> Items, int TotalCount)> GetUsersPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? role = null,
            bool? isActive = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true);
    }
}
