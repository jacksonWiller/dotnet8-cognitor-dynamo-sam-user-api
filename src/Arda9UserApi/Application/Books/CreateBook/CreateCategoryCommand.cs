using Ardalis.Result;
using MediatR;
namespace Arda9UserApi.Application.Books.CreateBook;
public class CreateBookCommand : IRequest<Result<CreateBookResponse>>
{
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public List<string>? Authors { get; set; }

}
