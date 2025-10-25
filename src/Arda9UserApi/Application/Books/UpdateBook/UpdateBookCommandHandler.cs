using Arda9UserApi.Repositories;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result<UpdateBookResponse>>
{
    private readonly IValidator<UpdateBookCommand> _validator;
    private readonly IBookRepository _bookRepository;

    public UpdateBookCommandHandler(
        IValidator<UpdateBookCommand> validator,
        IBookRepository bookRepository
    )
    {
        _validator = validator;
        _bookRepository = bookRepository;
    }

    public async Task<Result<UpdateBookResponse>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UpdateBookResponse>.Invalid(validationResult.AsErrors());
        }

        var existingBook = await _bookRepository.GetByIdAsync(request.Id);
        if (existingBook == null)
        {
            return Result<UpdateBookResponse>.NotFound($"Book with Id {request.Id} not found");
        }

        // Atualiza apenas os campos que foram fornecidos
        if (request.Title != null)
        {
            existingBook.Title = request.Title;
        }

        if (request.ISBN != null)
        {
            existingBook.ISBN = request.ISBN;
        }

        if (request.Authors != null)
        {
            existingBook.Authors = request.Authors;
        }

        var updateSuccess = await _bookRepository.UpdateAsync(existingBook);
        if (!updateSuccess)
        {
            return Result<UpdateBookResponse>.Error();
        }

        var response = new UpdateBookResponse
        {
            Id = existingBook.Id,
            Title = existingBook.Title,
            ISBN = existingBook.ISBN,
            Authors = existingBook.Authors
        };

        return Result<UpdateBookResponse>.Success(response, "Book updated successfully.");
    }
}
