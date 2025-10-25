using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.BookAggregate.Events.Categories;

public class CategoryDeletedEvent(Category category) : BaseEvent
{
    public Category Category { get; private init; } = category;
}
