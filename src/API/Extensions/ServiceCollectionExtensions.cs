using Microsoft.EntityFrameworkCore;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Data.Repositories;
using ProductAPI.Application.Interfaces;
using ProductAPI.Application.Services;
using ProductAPI.Infrastructure.Identity;
using ProductAPI.Infrastructure.Logging;
using ProductAPI.Infrastructure.Authorization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductAPI.API.Extensions;

/// <summary>
/// Service collection extensions for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add infrastructure services
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Logging
        services.AddApplicationLogging();

        // Add Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();

        // Add Authentication Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();

        // Add Authorization Services
        services.AddScoped<IAuthorizationPolicyService, AuthorizationPolicyService>();

        // Configure JWT Authentication
        AddJwtAuthentication(services, configuration);

        return services;
    }

    /// <summary>
    /// Configure JWT Authentication
    /// </summary>
    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configure Authorization with Infrastructure service
        services.AddAuthorization(options =>
        {
            var authorizationPolicyService = services.BuildServiceProvider().GetRequiredService<IAuthorizationPolicyService>();
            authorizationPolicyService.ConfigurePolicies(options);
        });
    }

    /// <summary>
    /// Add application services
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(ProductAPI.Application.Mapping.MappingProfile));

        // Add FluentValidation
        services.AddValidatorsFromAssemblyContaining<ProductAPI.Application.Validators.CreateProductDtoValidator>();

        // Add Application Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
