namespace Catalog.Domain.Entities.BookAggregate.Events.Books;

public class BookCreatedEvent(Book book) : BookBaseEvent(book)
{
}
