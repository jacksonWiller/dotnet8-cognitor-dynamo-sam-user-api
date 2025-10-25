using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Arda9UserApi.Application.DTOs;
using AutoMapper;
using Catalog.Domain.Entities.BookAggregate;

namespace Arda9UserApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDynamoDBContext context;
    private readonly ILogger<BookRepository> logger;
    private readonly IMapper mapper;

    public BookRepository(IDynamoDBContext context, ILogger<BookRepository> logger, IMapper mapper)
    {
        this.context = context;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<bool> CreateAsync(Book book)
    {
        try
        {
            // Mapeia a entidade de domínio para o DTO
            var bookDto = mapper.Map<BookDto>(book);
            bookDto.CreatedAt = DateTime.UtcNow;

            await context.SaveAsync(bookDto);
            logger.LogInformation("Book {Id} is added", book.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to persist to DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Book book)
    {
        bool result;
        try
        {
            // Deleta o book
            await context.DeleteAsync<BookDto>(book.Id);

            // Tenta recuperar o book deletado. Deve retornar null.
            BookDto? deletedBook = await context.LoadAsync<BookDto>(book.Id, new DynamoDBContextConfig
            {
                ConsistentRead = true
            });

            result = deletedBook == null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to delete book from DynamoDb Table");
            result = false;
        }

        if (result) logger.LogInformation("Book {Id} is deleted", book.Id);

        return result;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        if (book == null) return false;

        try
        {
            // Mapeia a entidade de domínio para o DTO
            var bookDto = mapper.Map<BookDto>(book);
            bookDto.UpdatedAt = DateTime.UtcNow;

            await context.SaveAsync(bookDto);
            logger.LogInformation("Book {Id} is updated", book.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to update book from DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        try
        {
            var bookDto = await context.LoadAsync<BookDto>(id);

            if (bookDto == null) return null;

            // Mapeia o DTO para a entidade de domínio
            return mapper.Map<Book>(bookDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to get book from DynamoDb Table");
            return null;
        }
    }

    public async Task<IList<Book>> GetBooksAsync(int limit = 10)
    {
        var result = new List<Book>();

        try
        {
            if (limit <= 0)
            {
                return result;
            }

            var filter = new ScanFilter();
            filter.AddCondition("Id", ScanOperator.IsNotNull);
            var scanConfig = new ScanOperationConfig()
            {
                Limit = limit,
                Filter = filter
            };
            var queryResult = context.FromScanAsync<BookDto>(scanConfig);

            do
            {
                var dtos = await queryResult.GetNextSetAsync();

                // Mapeia os DTOs para entidades de domínio
                var books = mapper.Map<List<Book>>(dtos);
                result.AddRange(books);
            }
            while (!queryResult.IsDone && result.Count < limit);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to list books from DynamoDb Table");
            return new List<Book>();
        }

        return result;
    }
}