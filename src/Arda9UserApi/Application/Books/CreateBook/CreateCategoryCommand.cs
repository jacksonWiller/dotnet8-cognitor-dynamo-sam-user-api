using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Books.CreateBook;

public class CreateBookCommand : IRequest<Result<CreateBookResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public List<Guid>? CategoryIds { get; set; }
}
