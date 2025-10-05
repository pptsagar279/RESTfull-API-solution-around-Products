namespace ProductAPI.Application.DTOs;

/// <summary>
/// Item DTO for API responses
/// </summary>
public class ItemDto
{
    public int ItemId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
