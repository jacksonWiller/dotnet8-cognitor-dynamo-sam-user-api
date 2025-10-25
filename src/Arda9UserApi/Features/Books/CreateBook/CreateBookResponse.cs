using System;
using Arda9UserApi.Core.CQRS;

namespace Arda9UserApi.Features.Books.CreateBook;

public class CreateBookResponse : IResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public List<string>? Authors { get; set; }
}
