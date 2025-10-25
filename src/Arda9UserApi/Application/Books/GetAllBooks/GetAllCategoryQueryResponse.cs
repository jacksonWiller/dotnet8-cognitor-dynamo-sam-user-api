using Arda9UserApi.Application.DTOs;
using Ardalis.Result;

namespace Arda9UserApi.Application.Books.GetAllBooks;

public class GetAllBooksQueryResponse
{
    public List<BookDto> Books { get; set; } = [];
    public int Count { get; set; }
}
