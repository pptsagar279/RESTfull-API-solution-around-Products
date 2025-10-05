using FluentValidation;
using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Validators;

/// <summary>
/// Validator for UpdateItemDto
/// </summary>
public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}
