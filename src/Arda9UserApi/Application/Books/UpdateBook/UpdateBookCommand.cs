using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommand : IRequest<Result<UpdateBookResponse>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? SKU { get; set; }
    public string? Brand { get; set; }
    public List<Guid>? CategoryIds { get; set; }
}
