namespace ProductAPI.Domain.Exceptions;

/// <summary>
/// Exception thrown when an item is not found
/// </summary>
public class ItemNotFoundException : DomainException
{
    public ItemNotFoundException(int itemId) 
        : base($"Item with ID {itemId} was not found.")
    {
    }
}
