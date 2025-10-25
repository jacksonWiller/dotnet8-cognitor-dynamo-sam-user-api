using Arda9UserApi.Entities;

namespace Arda9UserApi.Features.Books.GetAllBooks;

public class GetAllBooksQueryResponse 
{
    public List<Book> Books { get; set; } = [];
    public int Count { get; set; }
}
