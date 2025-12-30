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

        public async Task<IEnumerable<OrderListDto>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Getting all orders");
                var orders = await _unitOfWork.Orders.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} orders", orders.Count());
                return _mapper.Map<IEnumerable<OrderListDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting order with ID: {OrderId}", id);
                var order = await _unitOfWork.Orders.GetWithDetailsAsync(id);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved order: {OrderId}", id);
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order with ID: {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderListDto>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Getting orders for user: {UserId}", userId);
                var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
                _logger.LogInformation("Retrieved {Count} orders for user {UserId}", orders.Count(), userId);
                return _mapper.Map<IEnumerable<OrderListDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderListDto>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                _logger.LogInformation("Getting orders with status: {Status}", status);
                var orders = await _unitOfWork.Orders.GetByStatusAsync(status);
                _logger.LogInformation("Retrieved {Count} orders with status {Status}", orders.Count(), status);
                return _mapper.Map<IEnumerable<OrderListDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders with status: {Status}", status);
                throw;
            }
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
                    _logger.LogWarning("Cannot create order - cart is empty for user: {UserId}", userId);
                    throw new InvalidOperationException("Cart is empty");
                }

                _logger.LogInformation("Processing {Count} cart items", cartItems.Count());

                // Calculate total
                decimal subtotal = cartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity);
                decimal discount = 0;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(createDto.CouponCode))
                {
                    _logger.LogInformation("Applying coupon: {CouponCode}", createDto.CouponCode);
                    var coupon = await _unitOfWork.Coupons.GetByCodeAsync(createDto.CouponCode);
                    if (coupon != null && await _unitOfWork.Coupons.ValidateCouponAsync(createDto.CouponCode))
                    {
                        discount = coupon.DiscountValue;
                        _logger.LogInformation("Coupon applied - Discount: {Discount:C}", discount);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid or expired coupon: {CouponCode}", createDto.CouponCode);
                    }
                }

                decimal total = subtotal - discount;
                _logger.LogInformation("Order totals - Subtotal: {Subtotal:C}, Discount: {Discount:C}, Shipping: {Shipping:C}, Total: {Total:C}",
                    subtotal, discount, total);

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

                _logger.LogInformation("Order created with ID: {OrderId}", order.OrderId);

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
                _logger.LogInformation("Order details created for order: {OrderId}", order.OrderId);

                // Clear cart
                await _unitOfWork.CartItems.ClearCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Cart cleared for user: {UserId}", userId);

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Order transaction committed successfully - Order ID: {OrderId}", order.OrderId);

                var createdOrder = await _unitOfWork.Orders.GetWithDetailsAsync(order.OrderId);
                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating order for user: {UserId} - Transaction rolled back", userId);
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                _logger.LogInformation("Updating order {OrderId} status to: {Status}", orderId, status);
                var result = await _unitOfWork.Orders.UpdateStatusAsync(orderId, status);

                if (result)
                {
                    _logger.LogInformation("Order status updated successfully - Order: {OrderId}, New Status: {Status}", orderId, status);
                }
                else
                {
                    _logger.LogWarning("Failed to update order status - Order {OrderId} not found", orderId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status - Order: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Cancelling order: {OrderId}", orderId);

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
                    _logger.LogInformation("Order cancelled successfully: {OrderId}", orderId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Calculating total revenue - StartDate: {StartDate}, EndDate: {EndDate}",
                    startDate?.ToString("yyyy-MM-dd") ?? "N/A", endDate?.ToString("yyyy-MM-dd") ?? "N/A");

                var revenue = await _unitOfWork.Orders.GetTotalRevenueAsync(startDate, endDate);
                _logger.LogInformation("Total revenue calculated: {Revenue:C}", revenue);
                return revenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue");
                throw;
            }
        }
    }
}