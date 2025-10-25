using FluentValidation;

namespace Arda9UserApi.Application.Books.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Book name is required.")
            .MaximumLength(200).WithMessage("Book name must be up to 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Book description is required.")
            .MaximumLength(1000).WithMessage("Book description must be up to 1000 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must be up to 50 characters.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand must be up to 100 characters.");
    }
}
