using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.BookAggregate.Events.Categories;

public class CategoryCreatedEvent(Category category) : BaseEvent
{
    public Category Category { get; private init; } = category;
}
