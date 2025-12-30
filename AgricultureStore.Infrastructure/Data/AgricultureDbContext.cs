using AgricultureStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Data
{
    public class AgricultureDbContext : DbContext
    {
        public AgricultureDbContext(DbContextOptions<AgricultureDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
                entity.Property(e => e.RoleName).HasMaxLength(100).IsRequired();
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserAddress configuration
            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.HasKey(e => e.AddressId);
                entity.Property(e => e.AddressId).ValueGeneratedOnAdd();
                entity.Property(e => e.AddressLine).HasMaxLength(255);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserAddresses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();
                entity.Property(e => e.CategoryName).HasMaxLength(150).IsRequired();

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
                entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
                entity.Property(e => e.SupplierName).HasMaxLength(150);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(e => e.CategoryId);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ProductVariant configuration
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(e => e.VariantId);
                entity.Property(e => e.VariantId).ValueGeneratedOnAdd();
                entity.Property(e => e.VariantName).HasMaxLength(150);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.ProductId);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.ProductVariants)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemId);
                entity.Property(e => e.CartItemId).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(pv => pv.CartItems)
                    .HasForeignKey(e => e.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Coupon configuration
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.CouponId);
                entity.Property(e => e.CouponId).ValueGeneratedOnAdd();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StartDate).HasColumnType("datetime2");
                entity.Property(e => e.EndDate).HasColumnType("datetime2");

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
                entity.Property(e => e.ShippingAddress).HasMaxLength(255);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.Note).HasMaxLength(255);
                entity.Property(e => e.OrderDate).HasColumnType("datetime2");

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Coupon)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CouponId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // OrderDetail configuration
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);
                entity.Property(e => e.OrderDetailId).ValueGeneratedOnAdd();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.OrderId);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(pv => pv.OrderDetails)
                    .HasForeignKey(e => e.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId);
                entity.Property(e => e.ReviewId).ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(e => e.ProductId);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add check constraint for Rating (1-5)
                entity.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5"));
            });
        }
    }
}