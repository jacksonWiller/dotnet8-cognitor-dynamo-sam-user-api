using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;

namespace Arda9UserApi.Features.Books.Commands;

public record CreateBookCommand(Book Book) : ICommand<CreateBookResponse>;

public record CreateBookResponse(bool Success, Book? Book, string? ErrorMessage);

public class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, CreateBookResponse>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(IBookRepository bookRepository, ILogger<CreateBookCommandHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<CreateBookResponse> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (command.Book == null)
            {
                return new CreateBookResponse(false, null, "Invalid input! Book not informed");
            }

            // Garantir que o livro tenha um ID
            if (command.Book.Id == Guid.Empty)
            {
                command.Book.Id = Guid.NewGuid();
            }

            var result = await _bookRepository.CreateAsync(command.Book);

            if (result)
            {
                _logger.LogInformation("Book created successfully with ID: {BookId}", command.Book.Id);
                return new CreateBookResponse(true, command.Book, null);
            }
            else
            {
                _logger.LogError("Failed to persist book with ID: {BookId}", command.Book.Id);
                return new CreateBookResponse(false, null, "Fail to persist");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book");
            return new CreateBookResponse(false, null, "Internal error occurred");
        }
    }
}