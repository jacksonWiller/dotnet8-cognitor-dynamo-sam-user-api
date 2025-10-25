using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanySlugChangedEvent : BaseEvent
{
    public Company Company { get; }
    public string OldSlug { get; }
    public string NewSlug { get; }

    public CompanySlugChangedEvent(Company company, string oldSlug, string newSlug)
    {
        Company = company;
        OldSlug = oldSlug;
        NewSlug = newSlug;
    }
}
