using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductAPI.Application.Interfaces;
using Serilog;

namespace ProductAPI.Infrastructure.Logging;

/// <summary>
/// Logging configuration and setup
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configure Serilog logging
    /// </summary>
    public static ILogger ConfigureSerilog(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/productapi-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    /// <summary>
    /// Add application logging services
    /// </summary>
    public static IServiceCollection AddApplicationLogging(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        
        return services;
    }
}

