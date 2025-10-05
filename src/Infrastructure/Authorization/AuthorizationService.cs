using Microsoft.AspNetCore.Authorization;
using ProductAPI.Infrastructure.Authorization.Constants;

namespace ProductAPI.Infrastructure.Authorization;

/// <summary>
/// Service for configuring authorization policies
/// </summary>
public class AuthorizationPolicyService : IAuthorizationPolicyService
{
    /// <inheritdoc/>
    public void ConfigurePolicies(AuthorizationOptions options)
    {      
        // Read access policy (any authenticated user)
        options.AddPolicy(Policies.ReadAccess, policy =>
            policy.RequireAuthenticatedUser());
        
        // Write access policy (Manager and Admin)
        options.AddPolicy(Policies.WriteAccess, policy =>
            policy.RequireRole(Roles.Manager, Roles.Admin));
        
        // Delete access policy (Admin only)
        options.AddPolicy(Policies.DeleteAccess, policy =>
            policy.RequireRole(Roles.Admin));
    }
}
