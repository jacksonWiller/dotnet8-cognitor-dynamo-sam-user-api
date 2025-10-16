using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Repositories;

namespace Arda9UserApi.Features.Books.Commands;

public record DeleteBookCommand(Guid Id) : ICommand<DeleteBookResponse>;

public record DeleteBookResponse(bool Success, string? ErrorMessage);

public class DeleteBookCommandHandler : ICommandHandler<DeleteBookCommand, DeleteBookResponse>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<DeleteBookCommandHandler> _logger;

    public DeleteBookCommandHandler(IBookRepository bookRepository, ILogger<DeleteBookCommandHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<DeleteBookResponse> HandleAsync(DeleteBookCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (command.Id == Guid.Empty)
            {
                return new DeleteBookResponse(false, "Invalid request payload");
            }

            var existingBook = await _bookRepository.GetByIdAsync(command.Id);
            if (existingBook == null)
            {
                return new DeleteBookResponse(false, $"Invalid input! No book found with id:{command.Id}");
            }

            var result = await _bookRepository.DeleteAsync(existingBook);
            
            if (result)
            {
                _logger.LogInformation("Book deleted successfully with ID: {BookId}", command.Id);
                return new DeleteBookResponse(true, null);
            }
            else
            {
                _logger.LogError("Failed to delete book with ID: {BookId}", command.Id);
                return new DeleteBookResponse(false, "Fail to delete");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book with ID: {BookId}", command.Id);
            return new DeleteBookResponse(false, "Internal error occurred");
        }
    }
}