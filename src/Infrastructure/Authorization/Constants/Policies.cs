namespace ProductAPI.Infrastructure.Authorization.Constants;

/// <summary>
/// Authorization policies constants
/// </summary>
public static class Policies
{    
    /// <summary>
    /// Policy for read operations (any authenticated user)
    /// </summary>
    public const string ReadAccess = "ReadAccess";
    
    /// <summary>
    /// Policy for write operations (manager and admin)
    /// </summary>
    public const string WriteAccess = "WriteAccess";
    
    /// <summary>
    /// Policy for delete operations (admin only)
    /// </summary>
    public const string DeleteAccess = "DeleteAccess";
}
