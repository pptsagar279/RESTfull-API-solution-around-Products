namespace ProductAPI.Infrastructure.Authorization.Constants;

/// <summary>
/// Application roles constants
/// </summary>
public static class Roles
{
    /// <summary>
    /// Administrator role - Full access to all operations
    /// </summary>
    public const string Admin = "Admin";
    
    /// <summary>
    /// Manager role - Can manage products and items, view reports
    /// </summary>
    public const string Manager = "Manager";
    
    /// <summary>
    /// User role - Can view products and items, limited operations
    /// </summary>
    public const string User = "User";
    
    /// <summary>
    /// Read-only role - Can only view data
    /// </summary>
    public const string ReadOnly = "ReadOnly";
    
    /// <summary>
    /// Get all available roles
    /// </summary>
    public static readonly string[] AllRoles = { Admin, Manager, User, ReadOnly };
}
