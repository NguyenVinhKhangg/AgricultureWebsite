# AgricultureBackEnd - API Layer

## 📌 Mô Tả

Đây là **Presentation Layer** (API Layer) của Agriculture Store, được tổ chức theo **Clean Architecture**.

## 🏗️ Cấu Trúc

```
AgricultureBackEnd/
├── Controllers/           # API Controllers
├── Middleware/            # Custom middleware (GlobalExceptionHandler, CorrelationId)
├── Properties/            # Launch settings
├── Logs/                  # Application logs (Serilog)
├── Program.cs            # Application entry point & DI configuration
├── appsettings.Example.json  # Configuration template (copy to appsettings.json)
├── Dockerfile            # Docker configuration
└── AgricultureBackEnd.http  # HTTP request samples
```

## ⚙️ Configuration Setup

**Quan trọng:** File `appsettings.json` chứa thông tin nhạy cảm và không được push lên Git.

1. Copy file template:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```

2. Cập nhật các giá trị trong `appsettings.json`:
   - `ConnectionStrings:DefaultConnection` - Connection string đến SQL Server
   - `JwtSettings:Key` - Secret key cho JWT (ít nhất 32 ký tự)
   - `Cors:AllowedOrigins` - Danh sách domain được phép

3. (Optional) Tạo `appsettings.Development.json` cho môi trường development

**⚠️ Không bao giờ commit file appsettings.json chứa thông tin thật!**

## 🔗 Dependencies

API Layer này phụ thuộc vào:
- **AgricultureStore.Application** - Business logic & Services
- **AgricultureStore.Infrastructure** - Data access & Repositories

## 📦 Packages

- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT Authentication
- `Serilog.AspNetCore` - Structured logging
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI documentation

## 🚀 Chạy Application

```bash
# Development
dotnet run

# Production
dotnet run --configuration Release
```

Application sẽ chạy tại:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📝 Lưu Ý

- ✅ Code đã được migrate sang Clean Architecture
- ✅ Tất cả business logic nằm trong Application layer
- ✅ Tất cả data access nằm trong Infrastructure layer
- ✅ Controllers chỉ chịu trách nhiệm HTTP request/response handling
