using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.BookAggregate.Events.Books;

public abstract class BookBaseEvent(Book book) : BaseEvent
{
    public Book Book { get; private init; } = book;
}
