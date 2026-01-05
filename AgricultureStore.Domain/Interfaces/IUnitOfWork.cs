namespace AgricultureStore.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserAddressRepository UserAddresses { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IProductVariantRepository ProductVariants { get; }
        ICartItemRepository CartItems { get; }
        IOrderRepository Orders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IReviewRepository Reviews { get; }
        ICouponRepository Coupons { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
