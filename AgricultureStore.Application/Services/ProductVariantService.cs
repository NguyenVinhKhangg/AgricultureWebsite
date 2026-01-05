using AgricultureStore.Application.DTOs.ProductVariantDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductVariantService> _logger;

        public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductVariantService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync()
        {
            _logger.LogDebug("Getting all product variants");
            var variants = await _unitOfWork.ProductVariants.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<ProductVariantDto?> GetVariantByIdAsync(int id)
        {
            _logger.LogDebug("Getting product variant with ID: {VariantId}", id);
            var variant = await _unitOfWork.ProductVariants.GetWithProductAsync(id);

            if (variant == null)
            {
                _logger.LogWarning("Product variant with ID {VariantId} not found", id);
                return null;
            }

            return _mapper.Map<ProductVariantDto>(variant);
        }

        public async Task<IEnumerable<ProductVariantDto>> GetByProductIdAsync(int productId)
        {
            _logger.LogDebug("Getting variants for product ID: {ProductId}", productId);
            var variants = await _unitOfWork.ProductVariants.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<IEnumerable<ProductVariantDto>> GetLowStockVariantsAsync(int threshold)
        {
            _logger.LogDebug("Getting low stock variants with threshold: {Threshold}", threshold);
            var variants = await _unitOfWork.ProductVariants.GetLowStockVariantsAsync(threshold);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
        }

        public async Task<ProductVariantDto> CreateVariantAsync(CreateProductVariantDto createDto)
        {
            _logger.LogInformation("Creating new product variant: {VariantName} for product {ProductId}",
                createDto.VariantName, createDto.ProductId);

            var variant = _mapper.Map<ProductVariant>(createDto);
            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product variant created - VariantId: {VariantId}, Name: {VariantName}",
                variant.VariantId, variant.VariantName);

            var createdVariant = await _unitOfWork.ProductVariants.GetWithProductAsync(variant.VariantId);
            return _mapper.Map<ProductVariantDto>(createdVariant);
        }

        public async Task<bool> UpdateVariantAsync(int id, UpdateProductVariantDto updateDto)
        {
            _logger.LogDebug("Updating product variant with ID: {VariantId}", id);

            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
            if (variant == null)
            {
                _logger.LogWarning("Product variant with ID {VariantId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, variant);
            await _unitOfWork.ProductVariants.UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product variant updated - VariantId: {VariantId}", id);
            return true;
        }

        public async Task<bool> DeleteVariantAsync(int id)
        {
            _logger.LogDebug("Soft deleting product variant with ID: {VariantId}", id);

            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
            if (variant == null)
            {
                _logger.LogWarning("Product variant with ID {VariantId} not found for deletion", id);
                return false;
            }

            variant.IsActive = false;
            await _unitOfWork.ProductVariants.UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product variant soft-deleted - VariantId: {VariantId}", id);
            return true;
        }

        public async Task<bool> UpdateStockAsync(int variantId, int quantity)
        {
            _logger.LogDebug("Updating stock for variant {VariantId}, quantity: {Quantity}", variantId, quantity);

            var result = await _unitOfWork.ProductVariants.UpdateStockAsync(variantId, quantity);

            if (!result)
            {
                _logger.LogWarning("Failed to update stock for variant {VariantId} - insufficient stock or variant not found", variantId);
                return false;
            }

            _logger.LogInformation("Stock updated - VariantId: {VariantId}, Quantity: {Quantity}", variantId, quantity);
            return true;
        }
    }
}