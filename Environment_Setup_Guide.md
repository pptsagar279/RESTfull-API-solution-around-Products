# Environment Setup Guide - Product API

## Prerequisites

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB or full SQL Server)
- **Docker Desktop** (optional)

## Quick Setup

### Option 1: Local Development
```bash
# 1. Setup database (if you don't have SQL Server)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Qwerty123456" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Run the API
cd src/API
dotnet run
```

**Access**: https://localhost:58754 (Swagger UI)

### Option 2: Docker
```bash
# From project root
docker-compose up --build
```

**Access**: http://localhost:5000

## Database Configuration

The app expects SQL Server with:
- **Server**: localhost,1433
- **Database**: ProductAPIDb  
- **Username**: sa
- **Password**: Qwerty123456

## Quick Commands

```bash
# Build and run
dotnet build && dotnet run --project src/API

# Run tests
dotnet test

# Docker setup
docker-compose up --build

# Check health
curl http://localhost:5000/health
```

## Troubleshooting

**Database Connection Failed?**
- Make sure SQL Server is running
- Check connection string in appsettings.json

**Port Already in Use?**
- Change ports in launchSettings.json
- Or kill process: `netstat -ano | findstr :58754`

**CORS Issues?**
- Add your frontend URL to CORS settings
- Or set `"AllowedOrigins": ["*"]` for development

## Testing

1. Open Swagger UI (https://localhost:58754)
2. Click "Authorize" 
3. Login with test credentials (see Authentication Guide)
4. Test the endpoints

**Test Users:**
- admin / password123 (Admin)
- manager / password123 (Manager) 
- user / password123 (User)
- readonly / password123 (ReadOnly)
