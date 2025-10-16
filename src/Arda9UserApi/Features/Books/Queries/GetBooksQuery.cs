using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;
using Arda9UserApi.Repositories;

namespace Arda9UserApi.Features.Books.Queries;

public record GetBooksQuery(int Limit = 10) : IQuery<GetBooksResponse>;

public record GetBooksResponse(bool Success, IEnumerable<Book>? Books, string? ErrorMessage);

public class GetBooksQueryHandler : IQueryHandler<GetBooksQuery, GetBooksResponse>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<GetBooksQueryHandler> _logger;

    public GetBooksQueryHandler(IBookRepository bookRepository, ILogger<GetBooksQueryHandler> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<GetBooksResponse> HandleAsync(GetBooksQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            if (query.Limit <= 0 || query.Limit > 100)
            {
                return new GetBooksResponse(false, null, "The limit should been between [1-100]");
            }

            var books = await _bookRepository.GetBooksAsync(query.Limit);
            
            _logger.LogInformation("Retrieved {Count} books with limit {Limit}", books.Count, query.Limit);
            return new GetBooksResponse(true, books, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books with limit: {Limit}", query.Limit);
            return new GetBooksResponse(false, null, "Internal error occurred");
        }
    }
}