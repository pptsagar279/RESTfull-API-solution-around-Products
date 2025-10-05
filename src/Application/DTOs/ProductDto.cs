namespace ProductAPI.Application.DTOs;

/// <summary>
/// Product DTO for API responses
/// </summary>
public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}
