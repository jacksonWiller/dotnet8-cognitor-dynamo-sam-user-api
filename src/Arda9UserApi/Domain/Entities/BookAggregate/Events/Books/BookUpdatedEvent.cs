namespace Catalog.Domain.Entities.BookAggregate.Events.Books;

public class BookUpdatedEvent(Book book) : BookBaseEvent(book)
{
}
