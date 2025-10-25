using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanyCreatedEvent : BaseEvent
{
    public Company Company { get; }

    public CompanyCreatedEvent(Company company)
    {
        Company = company;
    }
}
