# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY src/API/*.csproj ./src/API/
COPY src/Application/*.csproj ./src/Application/
COPY src/Domain/*.csproj ./src/Domain/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/

# Restore dependencies (only production projects)
RUN dotnet restore src/API/ProductAPI.API.csproj

# Copy the rest of the source code
COPY . .

# Build the application (only the API project)
RUN dotnet build src/API/ProductAPI.API.csproj -c Release --no-restore

# Publish the application
RUN dotnet publish src/API/ProductAPI.API.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables for local SQL Server connection
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "ProductAPI.API.dll"]
