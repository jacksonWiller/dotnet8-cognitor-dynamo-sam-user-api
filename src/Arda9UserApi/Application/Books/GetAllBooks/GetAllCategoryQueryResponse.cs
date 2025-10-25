using Arda9UserApi.Entities;
using Ardalis.Result;

namespace Arda9UserApi.Application.Books.GetAllBooks;

public class GetAllBooksQueryResponse
{
    public List<Book> Books { get; set; } = [];
    public int Count { get; set; }
}
