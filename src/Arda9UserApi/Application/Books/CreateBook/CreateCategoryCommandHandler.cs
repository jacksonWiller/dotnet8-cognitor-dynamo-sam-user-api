using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Result<CreateBookResponse>>
{
    private readonly IValidator<CreateBookCommand> _validator;
    private readonly IBookRepository _bookRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateBookCommandHandler(
        IValidator<CreateBookCommand> validator,
        IBookRepository bookRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<CreateBookResponse>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateBookResponse>.Invalid(validationResult.AsErrors());
        }

        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            Authors = request.Authors
        };

        await _bookRepository.CreateAsync(book);

        var response = new CreateBookResponse
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Authors = book.Authors
        };

        return Result<CreateBookResponse>.Success(response, "Product created successfully.");
    }
}