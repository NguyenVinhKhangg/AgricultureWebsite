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
        private readonly ILogger<ProductVariantService> _logger;

        public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductVariantService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all product variants");
                var variants = await _unitOfWork.ProductVariants.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} product variants", variants.Count());
                return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product variants");
                throw;
            }
        }

        public async Task<ProductVariantDto?> GetVariantByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting product variant with ID: {VariantId}", id);
                var variant = await _unitOfWork.ProductVariants.GetWithProductAsync(id);

                if (variant == null)
                {
                    _logger.LogWarning("Product variant with ID {VariantId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved product variant: {VariantName} (ID: {VariantId})",
                    variant.VariantName, id);
                return _mapper.Map<ProductVariantDto>(variant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product variant with ID: {VariantId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductVariantDto>> GetByProductIdAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Getting variants for product ID: {ProductId}", productId);
                var variants = await _unitOfWork.ProductVariants.GetByProductIdAsync(productId);
                _logger.LogInformation("Retrieved {Count} variants for product {ProductId}", variants.Count(), productId);
                return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting variants for product ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductVariantDto>> GetLowStockVariantsAsync(int threshold)
        {
            try
            {
                _logger.LogInformation("Getting low stock variants with threshold: {Threshold}", threshold);
                var variants = await _unitOfWork.ProductVariants.GetLowStockVariantsAsync(threshold);
                _logger.LogInformation("Found {Count} low stock variants", variants.Count());
                return _mapper.Map<IEnumerable<ProductVariantDto>>(variants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock variants");
                throw;
            }
        }

        public async Task<ProductVariantDto> CreateVariantAsync(CreateProductVariantDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new product variant: {VariantName} for product {ProductId}",
                    createDto.VariantName, createDto.ProductId);

                var variant = _mapper.Map<ProductVariant>(createDto);
                await _unitOfWork.ProductVariants.AddAsync(variant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product variant created successfully with ID: {VariantId}", variant.VariantId);

                var createdVariant = await _unitOfWork.ProductVariants.GetWithProductAsync(variant.VariantId);
                return _mapper.Map<ProductVariantDto>(createdVariant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product variant: {VariantName}", createDto.VariantName);
                throw;
            }
        }

        public async Task<bool> UpdateVariantAsync(int id, UpdateProductVariantDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating product variant with ID: {VariantId}", id);

                var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
                if (variant == null)
                {
                    _logger.LogWarning("Product variant with ID {VariantId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, variant);
                await _unitOfWork.ProductVariants.UpdateAsync(variant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product variant updated successfully: {VariantId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product variant with ID: {VariantId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteVariantAsync(int id)
        {
            try
            {
                _logger.LogInformation("Soft deleting product variant with ID: {VariantId}", id);

                var variant = await _unitOfWork.ProductVariants.GetByIdAsync(id);
                if (variant == null)
                {
                    _logger.LogWarning("Product variant with ID {VariantId} not found for deletion", id);
                    return false;
                }

                variant.IsActive = false;
                await _unitOfWork.ProductVariants.UpdateAsync(variant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product variant soft-deleted successfully: {VariantId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product variant with ID: {VariantId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int variantId, int quantity)
        {
            try
            {
                _logger.LogInformation("Updating stock for variant {VariantId}, quantity: {Quantity}", variantId, quantity);

                var result = await _unitOfWork.ProductVariants.UpdateStockAsync(variantId, quantity);

                if (!result)
                {
                    _logger.LogWarning("Failed to update stock for variant {VariantId} - insufficient stock or variant not found", variantId);
                    return false;
                }

                _logger.LogInformation("Stock updated successfully for variant {VariantId}", variantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for variant {VariantId}", variantId);
                throw;
            }
        }
    }
}