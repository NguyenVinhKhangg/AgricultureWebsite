using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IUserAddressRepository : IRepository<UserAddress>
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId);
        Task<UserAddress?> GetDefaultAddressAsync(int userId);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
}
