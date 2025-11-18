using AgricultureBackEnd.DTOs.UserAddressDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserAddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserAddressDto>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _unitOfWork.UserAddresses.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
        }

        public async Task<UserAddressDto?> GetAddressByIdAsync(int id)
        {
            var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);
            return address == null ? null : _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto?> GetDefaultAddressAsync(int userId)
        {
            var address = await _unitOfWork.UserAddresses.GetDefaultAddressAsync(userId);
            return address == null ? null : _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto> CreateAddressAsync(int userId, CreateUserAddressDto createDto)
        {
            var address = _mapper.Map<UserAddress>(createDto);
            address.UserId = userId;

            await _unitOfWork.UserAddresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            if (createDto.IsDefault)
            {
                await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, address.AddressId);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<bool> UpdateAddressAsync(int id, UpdateUserAddressDto updateDto)
        {
            var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);
            if (address == null) return false;

            _mapper.Map(updateDto, address);
            await _unitOfWork.UserAddresses.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            if (updateDto.IsDefault == true)
            {
                await _unitOfWork.UserAddresses.SetDefaultAddressAsync(address.UserId, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            await _unitOfWork.UserAddresses.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            return await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, addressId);
        }
    }
}