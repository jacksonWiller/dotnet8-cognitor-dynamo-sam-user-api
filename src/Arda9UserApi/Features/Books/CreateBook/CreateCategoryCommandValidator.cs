using Arda9UserApi.Features.Books.CreateBook;
using FluentValidation;

namespace Catalog.Application.Categories.CreateCategory;
public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required.")
            .MaximumLength(200).WithMessage("Book title must be up to 200 characters.");

        RuleFor(x => x.ISBN)
            .Matches(@"^[\d-]{10,17}$").WithMessage("ISBN must be a valid format.")
            .When(x => !string.IsNullOrEmpty(x.ISBN));
    }
}
