namespace ProductAPI.Domain.Exceptions;

/// <summary>
/// Exception thrown when a product is not found
/// </summary>
public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int productId) 
        : base($"Product with ID {productId} was not found.")
    {
    }
}
