# Agriculture Store - Backend API

E-commerce backend application built with **Clean Architecture** principles using .NET 8.

## 🏗️ Architecture

```
AgricultureStore/
├── AgricultureStore.Domain/           # Core business entities & interfaces
├── AgricultureStore.Application/      # Business logic & use cases
├── AgricultureStore.Infrastructure/   # Data access & external services
└── AgricultureBackEnd/                # API layer (Controllers)
```

## 🚀 Quick Start

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API
cd AgricultureBackEnd
dotnet run
```

## 🔧 Tech Stack

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **Serilog** - Logging
- **Swagger** - API documentation
- **BCrypt** - Password hashing

## 📦 Projects

| Project | Description |
|---------|-------------|
| **Domain** | Entities, Repository interfaces (No dependencies) |
| **Application** | Services, DTOs, Business logic |
| **Infrastructure** | EF Core, Repositories, Data access |
| **API** | Controllers, HTTP handling |

## 🔗 API Endpoints

- `https://localhost:5001/swagger` - Swagger UI
- `https://localhost:5001/api/products` - Products API
- `https://localhost:5001/api/users` - Users API
- `https://localhost:5001/api/orders` - Orders API
- `https://localhost:5001/api/cart` - Shopping Cart API

## 📝 License

Private project
