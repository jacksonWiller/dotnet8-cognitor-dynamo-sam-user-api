namespace Catalog.Domain.Entities.BookAggregate.Events.Books;

public class BookDeletedEvent(Book book) : BookBaseEvent(book)
{
}
