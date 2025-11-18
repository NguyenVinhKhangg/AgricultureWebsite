using AgricultureBackEnd.DTOs.OrderDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderListDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderListDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderListDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderListDto>>(orders);
        }

        public async Task<IEnumerable<OrderListDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _unitOfWork.Orders.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderListDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Get cart items
                var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
                if (!cartItems.Any())
                    throw new InvalidOperationException("Cart is empty");

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
                    }
                }

                decimal total = subtotal - discount ;

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = createDto.ShippingAddress,
                    TotalAmount = total,
                    //ShippingFee = createDto.ShippingFee,
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

                var createdOrder = await _unitOfWork.Orders.GetWithDetailsAsync(order.OrderId);
                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            return await _unitOfWork.Orders.UpdateStatusAsync(orderId, status);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.Status != "Pending")
                return false;

            return await _unitOfWork.Orders.UpdateStatusAsync(orderId, "Cancelled");
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _unitOfWork.Orders.GetTotalRevenueAsync(startDate, endDate);
        }
    }
}