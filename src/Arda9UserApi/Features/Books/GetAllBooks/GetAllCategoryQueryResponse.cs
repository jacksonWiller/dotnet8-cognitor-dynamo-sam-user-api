using Arda9UserApi.Core.CQRS;
using Arda9UserApi.Entities;

namespace Arda9UserApi.Features.Books.GetAllBooks;

public class GetAllBooksQueryResponse : IResponse
{
    public List<Book> Books { get; set; } = [];
    public int Count { get; set; }
}
