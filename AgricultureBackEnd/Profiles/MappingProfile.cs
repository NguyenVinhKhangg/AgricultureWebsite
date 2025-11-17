using AgricultureBackEnd.DTOs.CartDTOs;
using AgricultureBackEnd.DTOs.CategoryDTOs;
using AgricultureBackEnd.DTOs.CouponDTOs;
using AgricultureBackEnd.DTOs.OrderDTOs;
using AgricultureBackEnd.DTOs.ProductDTOs;
using AgricultureBackEnd.DTOs.ProductVariantDTOs;
using AgricultureBackEnd.DTOs.ReviewDTOs;
using AgricultureBackEnd.DTOs.UserAddressDTOs;
using AgricultureBackEnd.DTOs.UserDTOs;
using AgricultureBackEnd.Models;
using AutoMapper;

namespace AgricultureBackEnd.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Handle password hashing separately
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserAddress mappings
            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<CreateUserAddressDto, UserAddress>();
            CreateMap<UpdateUserAddressDto, UserAddress>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.CategoryName : null));
            CreateMap<Category, CategoryWithSubCategoriesDto>()
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariants));
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => src.ProductVariants.Any() ? src.ProductVariants.Min(v => v.Price) : 0))
                .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => src.ProductVariants.Any() ? src.ProductVariants.Max(v => v.Price) : 0));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ProductVariant mappings
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
            CreateMap<CreateProductVariantDto, ProductVariant>();
            CreateMap<UpdateProductVariantDto, ProductVariant>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // CartItem mappings
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.ProductName))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant.VariantName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ProductVariant.Product.ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariant.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.ProductVariant.Price * src.Quantity));
            CreateMap<AddToCartDto, CartItem>();

            // Coupon mappings
            CreateMap<Coupon, CouponDto>()
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src =>
                    src.IsActive &&
                    src.StartDate <= DateTime.UtcNow &&
                    src.EndDate >= DateTime.UtcNow));
            CreateMap<CreateCouponDto, Coupon>();
            CreateMap<UpdateCouponDto, Coupon>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<Order, OrderListDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.Quantity)));
            CreateMap<CreateOrderDto, Order>();

            // OrderDetail mappings
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.ProductName))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant.VariantName))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ProductVariant.Product.ImageUrl))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
