# Deployment Guide - Product API

## Deployment Options

### 1. Docker Deployment (Recommended)

#### Single Container Deployment
```bash
# Build and run
docker build -t productapi .
docker run -p 5000:80 -e ASPNETCORE_ENVIRONMENT=Production productapi
```

#### Docker Compose Deployment
```bash
# Production deployment
docker-compose -f docker-compose.prod.yml up -d
```

### 2. Traditional Server Deployment

#### Prerequisites
- Windows Server with IIS
- .NET 8 Runtime
- SQL Server
- Reverse Proxy (nginx/IIS)

#### Steps
```bash
# 1. Publish application
dotnet publish src/API -c Release -o ./publish

# 2. Deploy to server
# Copy publish folder to server
# Configure IIS/nginx
# Setup database
```

## Production Configuration

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ConnectionStrings__DefaultConnection=<production_db_connection>
JwtSettings__SecretKey=<strong_secret_key>
JwtSettings__Issuer=ProductAPI
JwtSettings__Audience=ProductAPIUsers
```

### Database Setup
```sql
-- Create production database
CREATE DATABASE ProductAPIDb;
-- Run migrations (automatic on startup)
```

### Security Configuration
```json
{
  "JwtSettings": {
    "SecretKey": "<strong_256_bit_key>",
    "Issuer": "ProductAPI",
    "Audience": "ProductAPIUsers",
    "ExpiryInHours": 1
  }
}
```

## Deployment Environments

### Development
- **Purpose**: Local development and testing
- **Database**: Local SQL Server
- **Authentication**: Development JWT settings
- **CORS**: Localhost origins

### Production
- **Purpose**: Live application
- **Database**: Production SQL Server
- **Authentication**: Production JWT settings
- **CORS**: Production domain origins

## Database Deployment

### SQL Server Setup
```sql
CREATE DATABASE ProductAPIDb;
```
### Connection String Examples
```bash
# Local SQL Server
Server=localhost,1433;Database=ProductAPIDb;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=true
```
### Health Check Endpoint
```bash
# Check application health
curl http://your-api.com/health
```

### Logging Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```
## Deployment Checklist

### Pre-Deployment
- [ ] Update JWT secret key
- [ ] Configure production database
- [ ] Set up SSL certificates

### Deployment
- [ ] Deploy application
- [ ] Run database
- [ ] Configure health checks
- [ ] Set up logging
- [ ] Test all endpoints

### Post-Deployment
- [ ] Verify health endpoint
- [ ] Test authentication
- [ ] Check logs

## Quick Deployment Commands

```bash
# Docker deployment
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f

# Update deployment
docker-compose pull
docker-compose up -d

# Rollback
docker-compose down
docker-compose up -d --scale productapi=0
```

## Troubleshooting

### Common Issues
- **Database connection failed**: Check connection string and database availability
- **JWT token invalid**: Verify JWT settings and secret key
- **Health check failing**: Check application logs and endpoint availability

### Logs Location
- **Docker**: `docker-compose logs productapi`
- **Local**: `./logs/` directory
