using Arda9UserApi.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.DeleteBook;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Result<DeleteBookResponse>>
{
    private readonly IValidator<DeleteBookCommand> _validator;
    private readonly IBookRepository _bookRepository;

    public DeleteBookCommandHandler(
        IValidator<DeleteBookCommand> validator,
        IBookRepository bookRepository
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
    }

    public async Task<Result<DeleteBookResponse>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<DeleteBookResponse>.Invalid(validationResult.AsErrors());
        }

        var existingBook = await _bookRepository.GetByIdAsync(request.Id);
        if (existingBook == null)
        {
            return Result<DeleteBookResponse>.NotFound($"Book with Id {request.Id} not found");
        }

        var deleteSuccess = await _bookRepository.DeleteAsync(existingBook);
        if (!deleteSuccess)
        {
            return Result<DeleteBookResponse>.Invalid();
        }

        var response = new DeleteBookResponse
        {
            Id = request.Id,
            Message = "Book deleted successfully"
        };

        return Result<DeleteBookResponse>.Success(response, "Book deleted successfully.");
    }
}
