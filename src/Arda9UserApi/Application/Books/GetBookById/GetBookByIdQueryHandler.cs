using Arda9UserApi.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<GetBookByIdQueryResponse>>
{
    private readonly IValidator<GetBookByIdQuery> _validator;
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(
        IValidator<GetBookByIdQuery> validator,
        IBookRepository bookRepository
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
    }

    public async Task<Result<GetBookByIdQueryResponse>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetBookByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var book = await _bookRepository.GetByIdAsync(request.Id);

        if (book == null)
        {
            return Result<GetBookByIdQueryResponse>.NotFound($"Book with ID {request.Id} not found.");
        }

        var response = new GetBookByIdQueryResponse { Book = book };

        return Result<GetBookByIdQueryResponse>.Success(response, "Book retrieved successfully.");
    }
}
