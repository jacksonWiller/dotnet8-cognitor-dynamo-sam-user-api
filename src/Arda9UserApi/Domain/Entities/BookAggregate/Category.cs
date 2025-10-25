using System.Collections.Generic;
using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.BookAggregate.Events.Categories;

namespace Catalog.Domain.Entities.BookAggregate;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Book> Books { get; private set; } = [];

    public bool _isDeleted { get; private set; } = false;

    public Category() {}

    public Category(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryCreatedEvent(this));
    }

    public void Update(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }

    public void AddBook(Book book)
    {
        book.Categories.Add(this);
        Books.Add(book);
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }

    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;
        AddDomainEvent(new CategoryDeletedEvent(this));
    }
}
