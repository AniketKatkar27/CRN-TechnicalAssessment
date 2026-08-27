using CRN.Application.DTOs;
using FluentValidation;

namespace CRN.Application.Validators;

public class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(255)
            .WithMessage("Product name cannot exceed 255 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Created by is required.")
            .MaximumLength(100)
            .WithMessage("Created by cannot exceed 100 characters.");
    }
}