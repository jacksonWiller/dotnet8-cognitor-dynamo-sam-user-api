using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;

namespace Arda9UserApi.Features.Books.Commands;

public record UpdateBookCommand(Guid Id, Book Book) : ICommand<UpdateBookResponse>;

public record UpdateBookResponse(bool Success, string? ErrorMessage);

public class UpdateBookCommandHandler : ICommandHandler<UpdateBookCommand, UpdateBookResponse>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<UpdateBookCommandHandler> _logger;

    public UpdateBookCommandHandler(IBookRepository bookRepository, ILogger<UpdateBookCommandHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<UpdateBookResponse> HandleAsync(UpdateBookCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (command.Id == Guid.Empty || command.Book == null)
            {
                return new UpdateBookResponse(false, "Invalid request payload");
            }

            // Verificar se o livro existe
            var existingBook = await _bookRepository.GetByIdAsync(command.Id);
            if (existingBook == null)
            {
                return new UpdateBookResponse(false, $"Invalid input! No book found with id:{command.Id}");
            }

            // Garantir que o ID seja preservado
            command.Book.Id = existingBook.Id;

            var result = await _bookRepository.UpdateAsync(command.Book);
            
            if (result)
            {
                _logger.LogInformation("Book updated successfully with ID: {BookId}", command.Id);
                return new UpdateBookResponse(true, null);
            }
            else
            {
                _logger.LogError("Failed to update book with ID: {BookId}", command.Id);
                return new UpdateBookResponse(false, "Fail to update");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book with ID: {BookId}", command.Id);
            return new UpdateBookResponse(false, "Internal error occurred");
        }
    }
}