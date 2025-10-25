using Catalog.Core.SharedKernel;

namespace Catalog.Domain.Entities.BookAggregate.Events.Categories;

public class CategoryUpdatedEvent(Category category) : BaseEvent
{
    public Category Category { get; private init; } = category;
}
