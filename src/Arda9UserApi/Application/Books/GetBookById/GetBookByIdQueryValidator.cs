using FluentValidation;

namespace Arda9UserApi.Application.Books.GetBookById;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty()
            .WithMessage("Book ID is required.");
    }
}
