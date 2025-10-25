using Catalog.Core.SharedKernel;

namespace Catalog.Domain.Entities.BookAggregate.Events.Books;

public abstract class BookBaseEvent(Book book) : BaseEvent
{
    public Book Book { get; private init; } = book;
}
