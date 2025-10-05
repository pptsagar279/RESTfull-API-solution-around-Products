using FluentValidation;
using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Validators;

/// <summary>
/// Validator for CreateItemDto
/// </summary>
public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}
