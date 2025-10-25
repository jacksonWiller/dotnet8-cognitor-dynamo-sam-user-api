using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanyUpdatedEvent : BaseEvent
{
    public Company Company { get; }

    public CompanyUpdatedEvent(Company company)
    {
        Company = company;
    }
}
