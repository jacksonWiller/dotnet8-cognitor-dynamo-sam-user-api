using FluentValidation;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title cannot be empty when provided")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters");
        });

        When(x => x.ISBN != null, () =>
        {
            RuleFor(x => x.ISBN)
                .MaximumLength(20)
                .WithMessage("ISBN cannot exceed 20 characters");
        });
    }
}
