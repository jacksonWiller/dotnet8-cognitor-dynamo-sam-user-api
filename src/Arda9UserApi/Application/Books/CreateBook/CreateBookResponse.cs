namespace Arda9UserApi.Application.Books.CreateBook;

public class CreateBookResponse 
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public List<string>? Authors { get; set; }
}
