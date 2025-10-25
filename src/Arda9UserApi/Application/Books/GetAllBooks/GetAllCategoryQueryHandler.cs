using Arda9UserApi.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.GetAllBooks;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, Result<GetAllBooksQueryResponse>>
{
    private readonly IValidator<GetAllBooksQuery> _validator;
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(
        IValidator<GetAllBooksQuery> validator,
        IBookRepository bookRepository
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
    }

    public async Task<Result<GetAllBooksQueryResponse>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetAllBooksQueryResponse>.Invalid(validationResult.AsErrors());
        }


        var books = await _bookRepository.GetBooksAsync(request.Limit);
        var response = new GetAllBooksQueryResponse() { Books = books.ToList(), Count = books.Count() };

        return Result<GetAllBooksQueryResponse>.Success(response, "Categorys retrieved successfully.");
    }
}