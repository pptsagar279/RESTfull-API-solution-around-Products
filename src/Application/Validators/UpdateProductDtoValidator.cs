using FluentValidation;
using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Validators;

/// <summary>
/// Validator for UpdateProductDto
/// </summary>
public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .Length(1, 255)
            .WithMessage("Product name must be between 1 and 255 characters");

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .WithMessage("ModifiedBy is required")
            .Length(1, 100)
            .WithMessage("ModifiedBy must be between 1 and 100 characters");
    }
}
