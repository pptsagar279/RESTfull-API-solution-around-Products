using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Application.DTOs;

/// <summary>
/// DTO for updating an item
/// </summary>
public class UpdateItemDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}
