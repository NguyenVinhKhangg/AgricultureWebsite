using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.OrderDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<OrderListDto>> GetAllOrdersAsync(OrderFilterParams? filterParams = null)
        {
            filterParams ??= new OrderFilterParams();

            _logger.LogDebug("Getting orders - Page: {PageNumber}, Size: {PageSize}, Status: {Status}",
                filterParams.PageNumber, filterParams.PageSize, filterParams.Status);

            var (orders, totalCount) = await _unitOfWork.Orders.GetOrdersPagedAsync(
                filterParams.PageNumber,
                filterParams.PageSize,
                filterParams.Status,
                filterParams.UserId,
                filterParams.FromDate,
                filterParams.ToDate,
                filterParams.SortBy ?? "OrderDate",
                filterParams.SortDescending);

            var orderDtos = _mapper.Map<IEnumerable<OrderListDto>>(orders);

            return new PagedResult<OrderListDto>(orderDtos, totalCount, filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            _logger.LogDebug("Getting order with ID: {OrderId}", id);
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return null;
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<PagedResult<OrderListDto>> GetOrdersByUserIdAsync(int userId, PaginationParams? paginationParams = null)
        {
            paginationParams ??= new PaginationParams();

            _logger.LogDebug("Getting orders for user - UserId: {UserId}, Page: {PageNumber}",
                userId, paginationParams.PageNumber);

            var (orders, totalCount) = await _unitOfWork.Orders.GetByUserIdPagedAsync(
                userId,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var orderDtos = _mapper.Map<IEnumerable<OrderListDto>>(orders);

            return new PagedResult<OrderListDto>(orderDtos, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PagedResult<OrderListDto>> GetOrdersByStatusAsync(string status, PaginationParams? paginationParams = null)
        {
            paginationParams ??= new PaginationParams();

            _logger.LogDebug("Getting orders with status: {Status}, Page: {PageNumber}", 
                status, paginationParams.PageNumber);

            var filterParams = new OrderFilterParams
            {
                Status = status,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };

            return await GetAllOrdersAsync(filterParams);
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Creating order for user: {UserId}", userId);

                // Get cart items
                var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                // Calculate total
                decimal subtotal = cartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity);
                decimal discount = 0;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(createDto.CouponCode))
                {
                    var coupon = await _unitOfWork.Coupons.GetByCodeAsync(createDto.CouponCode);
                    if (coupon != null && await _unitOfWork.Coupons.ValidateCouponAsync(createDto.CouponCode))
                    {
                        discount = coupon.DiscountValue;
                        _logger.LogDebug("Coupon {CouponCode} applied - Discount: {Discount}", createDto.CouponCode, discount);
                    }
                }

                decimal total = subtotal - discount;

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = createDto.ShippingAddress,
                    TotalAmount = total,
                    ShippingFee = 0,
                    Status = "Pending",
                    PaymentMethod = createDto.PaymentMethod,
                    Note = createDto.Note
                };

                if (!string.IsNullOrEmpty(createDto.CouponCode))
                {
                    var coupon = await _unitOfWork.Coupons.GetByCodeAsync(createDto.CouponCode);
                    if (coupon != null)
                        order.CouponId = coupon.CouponId;
                }

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Create order details
                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        VariantId = cartItem.VariantId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.ProductVariant.Price
                    };
                    await _unitOfWork.OrderDetails.AddAsync(orderDetail);
                }

                await _unitOfWork.SaveChangesAsync();

                // Clear cart
                await _unitOfWork.CartItems.ClearCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                
                _logger.LogInformation("Order created successfully - OrderId: {OrderId}, UserId: {UserId}, Total: {Total}",
                    order.OrderId, userId, total);

                var createdOrder = await _unitOfWork.Orders.GetWithDetailsAsync(order.OrderId);
                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("Order creation rolled back for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            _logger.LogDebug("Updating order {OrderId} status to: {Status}", orderId, status);
            var result = await _unitOfWork.Orders.UpdateStatusAsync(orderId, status);

            if (result)
            {
                _logger.LogInformation("Order status updated - OrderId: {OrderId}, Status: {Status}", orderId, status);
            }
            else
            {
                _logger.LogWarning("Order {OrderId} not found for status update", orderId);
            }

            return result;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            _logger.LogDebug("Cancelling order: {OrderId}", orderId);

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for cancellation", orderId);
                return false;
            }

            if (order.Status != "Pending")
            {
                _logger.LogWarning("Cannot cancel order {OrderId} - Status is {Status}", orderId, order.Status);
                return false;
            }

            var result = await _unitOfWork.Orders.UpdateStatusAsync(orderId, "Cancelled");
            if (result)
            {
                _logger.LogInformation("Order cancelled - OrderId: {OrderId}", orderId);
            }

            return result;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            _logger.LogDebug("Calculating total revenue - StartDate: {StartDate}, EndDate: {EndDate}",
                startDate?.ToString("yyyy-MM-dd") ?? "N/A", endDate?.ToString("yyyy-MM-dd") ?? "N/A");

            return await _unitOfWork.Orders.GetTotalRevenueAsync(startDate, endDate);
        }
    }
}