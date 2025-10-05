using Microsoft.AspNetCore.Authorization;
using ProductAPI.Infrastructure.Authorization.Constants;

namespace ProductAPI.Infrastructure.Authorization;

/// <summary>
/// Read access authorization attribute (any authenticated user)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ReadAccessAttribute : AuthorizeAttribute
{
    public ReadAccessAttribute()
    {
        // Any authenticated user can read
        Policy = Policies.ReadAccess;
    }
}

/// <summary>
/// Write access authorization attribute (Manager and Admin)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class WriteAccessAttribute : AuthorizeAttribute
{
    public WriteAccessAttribute()
    {
        Policy = Policies.WriteAccess;
    }
}

/// <summary>
/// Delete access authorization attribute (Admin only)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DeleteAccessAttribute : AuthorizeAttribute
{
    public DeleteAccessAttribute()
    {
        Policy = Policies.DeleteAccess;
    }
}
