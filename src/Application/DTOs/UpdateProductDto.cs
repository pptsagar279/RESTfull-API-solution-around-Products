using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Application.DTOs;

/// <summary>
/// DTO for updating a product
/// </summary>
public class UpdateProductDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ModifiedBy { get; set; } = string.Empty;
}
