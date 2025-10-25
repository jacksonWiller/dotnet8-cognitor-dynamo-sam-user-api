using FluentValidation;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty when provided")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty when provided")
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters");
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be greater than or equal to 0");
        });

        When(x => x.StockQuantity.HasValue, () =>
        {
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock quantity must be greater than or equal to 0");
        });

        When(x => x.SKU != null, () =>
        {
            RuleFor(x => x.SKU)
                .NotEmpty()
                .WithMessage("SKU cannot be empty when provided")
                .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters");
        });

        When(x => x.Brand != null, () =>
        {
            RuleFor(x => x.Brand)
                .NotEmpty()
                .WithMessage("Brand cannot be empty when provided")
                .MaximumLength(100)
                .WithMessage("Brand cannot exceed 100 characters");
        });
    }
}
