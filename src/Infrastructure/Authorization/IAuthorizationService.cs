
namespace ProductAPI.Infrastructure.Authorization;

/// <summary>
/// Interface for authorization policy configuration service
/// </summary>
public interface IAuthorizationPolicyService
{
    /// <summary>
    /// Configure authorization policies
    /// </summary>
    /// <param name="options">Authorization options</param>
    void ConfigurePolicies(Microsoft.AspNetCore.Authorization.AuthorizationOptions options);
}
