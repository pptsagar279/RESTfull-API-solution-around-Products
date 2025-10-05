using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Application.DTOs;

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string CreatedBy { get; set; } = string.Empty;
}
