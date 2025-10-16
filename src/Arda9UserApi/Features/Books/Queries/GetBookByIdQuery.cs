using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;

namespace Arda9UserApi.Features.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdResponse>;

public record GetBookByIdResponse(bool Success, Book? Book, string? ErrorMessage, bool NotFound = false);

public class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, GetBookByIdResponse>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<GetBookByIdQueryHandler> _logger;

    public GetBookByIdQueryHandler(IBookRepository bookRepository, ILogger<GetBookByIdQueryHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<GetBookByIdResponse> HandleAsync(GetBookByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            if (query.Id == Guid.Empty)
            {
                return new GetBookByIdResponse(false, null, "Invalid book ID");
            }

            var book = await _bookRepository.GetByIdAsync(query.Id);
            
            if (book == null)
            {
                _logger.LogInformation("Book not found with ID: {BookId}", query.Id);
                return new GetBookByIdResponse(false, null, "Book not found", true);
            }

            _logger.LogInformation("Book retrieved successfully with ID: {BookId}", query.Id);
            return new GetBookByIdResponse(true, book, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book with ID: {BookId}", query.Id);
            return new GetBookByIdResponse(false, null, "Internal error occurred");
        }
    }
}