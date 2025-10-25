using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Books.UpdateBook;

public class UpdateBookCommand : IRequest<Result<UpdateBookResponse>>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? ISBN { get; set; }
    public List<string>? Authors { get; set; }
}
