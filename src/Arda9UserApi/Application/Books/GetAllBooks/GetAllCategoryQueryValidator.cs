using FluentValidation;

namespace Arda9UserApi.Application.Books.GetAllBooks;
public class GetAllCategorysQueryValidator : AbstractValidator<GetAllBooksQuery>
{
    public GetAllCategorysQueryValidator()
    {
        //RuleFor(command => command.Id)
        //   .NotEmpty();
    }
}
