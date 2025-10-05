# Product API

A RESTful API for managing products and items with JWT authentication and role-based authorization.

## Features

- 🔐 **JWT Authentication** - Secure token-based authentication
- 👥 **Role-Based Authorization** - Admin, Manager, User, and ReadOnly roles
- 🏗️ **Clean Architecture** - Domain, Application, Infrastructure layers
- 🐳 **Docker Support** - Containerized deployment
- 📊 **Health Checks** - Built-in monitoring endpoints
- 📝 **API Documentation** - Swagger/OpenAPI integration
- 🧪 **Unit Tests** - Comprehensive test coverage

## Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full SQL Server)
- Docker Desktop (optional)

### Local Development
```bash
# 1. Clone the repository
git clone <your-repo-url>
cd SAGAR_LLD

# 2. Setup database (if needed)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Qwerty123456" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# 3. Run the API
cd src/API
dotnet run
```

**Access**: https://localhost:58754 (Swagger UI)

### Docker Deployment
```bash
# Build and run with Docker Compose
docker-compose up --build
```

**Access**: http://localhost:5000

## API Endpoints

### Authentication
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refreshtoken` - Refresh access token

### Products
- `GET /api/v1/products` - Get all products
- `POST /api/v1/products` - Create product (Manager/Admin)
- `PUT /api/v1/products/{id}` - Update product (Manager/Admin)
- `DELETE /api/v1/products/{id}` - Delete product (Admin only)

### Items
- `GET /api/v1/items` - Get all items
- `POST /api/v1/items` - Create item (Manager/Admin)
- `PUT /api/v1/items/{id}` - Update item (Manager/Admin)
- `DELETE /api/v1/items/{id}` - Delete item (Admin only)

## Test Users

| Username | Password | Role | Permissions |
|----------|----------|------|-------------|
| admin | password123 | Admin | Full access |
| manager | password123 | Manager | Read/Write |
| user | password123 | User | Limited access |
| readonly | password123 | ReadOnly | Read only |

## Project Structure

```
SAGAR_LLD/
├── src/
│   ├── API/                 # Web API layer
│   ├── Application/         # Business logic & DTOs
│   ├── Domain/             # Entities & exceptions
│   └── Infrastructure/     # Data access & external services
├── tests/                  # Unit and integration tests
├── docker-compose.yml      # Docker configuration
├── Dockerfile             # Container build instructions
└── ProductAPI.sln         # Visual Studio solution
```

## Technology Stack

- **.NET 8** - Web API framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Bearer** - Authentication
- **Serilog** - Logging
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Docker** - Containerization

## Development

### Running Tests
```bash
dotnet test
```

### Building
```bash
dotnet build
```

### Database Migrations
The application automatically creates the database and tables on first run.

## Documentation

- [Authentication Guide](Authentication_Flow_Documentation.md) - JWT authentication flow
- [Environment Setup](Environment_Setup_Guide.md) - Development environment setup
- [Deployment Guide](Deployment_Guide.md) - Production deployment procedures

## Health Checks

- **Endpoint**: `/health`
- **Docker**: Built-in health check every 30 seconds

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request
