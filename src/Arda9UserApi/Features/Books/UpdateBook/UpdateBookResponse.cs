namespace Arda9UserApi.Features.Books.UpdateBook;

public class UpdateBookResponse 
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public List<string>? Authors { get; set; }
}
