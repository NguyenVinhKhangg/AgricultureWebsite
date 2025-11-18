using AgricultureBackEnd.DTOs.ProductVariantDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync()
        {
            var variants = await _unitOfWork.ProductVariants.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<ProductVariantDto?> GetVariantByIdAsync(int id)
        {
            var variant = await _unitOfWork.ProductVariants.GetWithProductAsync(id);
            return variant == null ? null : _mapper.Map<ProductVariantDto>(variant);
        }

        public async Task<IEnumerable<ProductVariantDto>> GetByProductIdAsync(int productId)
        {
            var variants = await _unitOfWork.ProductVariants.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<IEnumerable<ProductVariantDto>> GetLowStockVariantsAsync(int threshold)
        {
            var variants = await _unitOfWork.ProductVariants.GetLowStockVariantsAsync(threshold);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<ProductVariantDto> CreateVariantAsync(CreateProductVariantDto createDto)
        {
            var variant = _mapper.Map<ProductVariant>(createDto);
            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            var createdVariant = await _unitOfWork.ProductVariants.GetWithProductAsync(variant.VariantId);
            return _mapper.Map<ProductVariantDto>(createdVariant);
        }

        public async Task<bool> UpdateVariantAsync(int id, UpdateProductVariantDto updateDto)
        {
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
            if (variant == null) return false;

            _mapper.Map(updateDto, variant);
            await _unitOfWork.ProductVariants.UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVariantAsync(int id)
        {
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
            if (variant == null) return false;

            variant.IsActive = false;
            await _unitOfWork.ProductVariants.UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int variantId, int quantity)
        {
            return await _unitOfWork.ProductVariants.UpdateStockAsync(variantId, quantity);
        }
    }
}