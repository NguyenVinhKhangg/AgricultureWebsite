using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AgricultureDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IUserAddressRepository UserAddresses { get; }
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IProductVariantRepository ProductVariants { get; }
        public ICartItemRepository CartItems { get; }
        public IOrderRepository Orders { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IReviewRepository Reviews { get; }
        public ICouponRepository Coupons { get; }

        public UnitOfWork(
            AgricultureDbContext context,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserAddressRepository userAddressRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductVariantRepository productVariantRepository,
            ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IReviewRepository reviewRepository,
            ICouponRepository couponRepository)
        {
            _context = context;
            Users = userRepository;
            Roles = roleRepository;
            UserAddresses = userAddressRepository;
            Products = productRepository;
            Categories = categoryRepository;
            ProductVariants = productVariantRepository;
            CartItems = cartItemRepository;
            Orders = orderRepository;
            OrderDetails = orderDetailRepository;
            Reviews = reviewRepository;
            Coupons = couponRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}