using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.Repositories.Interface
{
    public interface IUserAddressRepository : IRepository<UserAddress>
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId);
        Task<UserAddress?> GetDefaultAddressAsync(int userId);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
}