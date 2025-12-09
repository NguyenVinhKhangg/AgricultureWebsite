using AgricultureBackEnd.Profiles;
using AgricultureBackEnd.Repositories.Implement;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Implement;
using AgricultureBackEnd.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AgricultureBackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "AgricultureBackEnd")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                    retainedFileCountLimit: 30)
                .CreateLogger();

            try
            {
                Log.Information("Starting Agriculture Backend API");

                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddDbContext<Data.AgricultureDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Auto Mapper Configurations
                builder.Services.AddAutoMapper(typeof(MappingProfile));

                // Repository Pattern
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
                builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
                builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
                builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
                builder.Services.AddScoped<ICouponRepository, CouponRepository>();

                // Unit of Work Pattern
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                // Service Layer
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<ICategoryService, CategoryService>();
                builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
                builder.Services.AddScoped<ICartService, CartService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IReviewService, ReviewService>();
                builder.Services.AddScoped<ICouponService, CouponService>();
                builder.Services.AddScoped<IUserAddressService, UserAddressService>();

                var app = builder.Build();

                // Add Serilog request logging
                app.UseSerilogRequestLogging();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Application started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}