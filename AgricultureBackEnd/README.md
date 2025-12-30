# AgricultureBackEnd - API Layer

## 📌 Mô Tả

Đây là **Presentation Layer** (API Layer) của Agriculture Store, được tổ chức theo **Clean Architecture**.

## 🏗️ Cấu Trúc

```
AgricultureBackEnd/
├── Controllers/           # API Controllers
├── Properties/            # Launch settings
├── Logs/                  # Application logs (Serilog)
├── Program.cs            # Application entry point & DI configuration
├── appsettings.json      # Configuration
├── Dockerfile            # Docker configuration
└── AgricultureBackEnd.http  # HTTP request samples
```

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
