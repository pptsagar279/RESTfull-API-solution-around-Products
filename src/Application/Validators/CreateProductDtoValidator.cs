using FluentValidation;
using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Validators;

/// <summary>
/// Validator for CreateProductDto
/// </summary>
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .Length(1, 255)
            .WithMessage("Product name must be between 1 and 255 characters");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("CreatedBy is required")
            .Length(1, 100)
            .WithMessage("CreatedBy must be between 1 and 100 characters");
    }
}
