using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Application.DTOs;

/// <summary>
/// DTO for creating a new item
/// </summary>
public class CreateItemDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}
