using System.Text;
using AgricultureBackEnd.Middleware;
using AgricultureStore.Application.Mappings;
using AgricultureStore.Application.Interfaces;
using AgricultureStore.Application.Services;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Infrastructure.Repositories;
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
                builder.Services.AddDbContext<AgricultureDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen( c=>
                {
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Agriculture Store API",
                        Version = "v1",
                        Description = "API documentation for the Agriculture Store backend."
                    });

                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                           Array.Empty<string>()
                        }
                    });
                });
                
                //Configure JWT Authentication
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });


                // Configure Authorize             
                builder.Services.AddAuthorization();

                // Configure CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("Development", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });

                    options.AddPolicy("Production", policy =>
                    {
                        policy.WithOrigins(
                                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>()
                              )
                              .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                              .WithHeaders("Authorization", "Content-Type", "Accept")
                              .AllowCredentials();
                    });
                });
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
                builder.Services.AddScoped<IAuthService, AuthService>();


                // Global exception handling
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddProblemDetails();
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
                
                // Use CORS based on environment
                if (app.Environment.IsDevelopment())
                {
                    app.UseCors("Development");
                }
                else
                {
                    app.UseCors("Production");
                }

                // Use global exception handler
                app.UseExceptionHandler();

                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Application started successfully");
                
                app.Lifetime.ApplicationStarted.Register(() =>
                {
                    Log.Information("Listening on: {Urls}", string.Join(", ", app.Urls));
                });
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