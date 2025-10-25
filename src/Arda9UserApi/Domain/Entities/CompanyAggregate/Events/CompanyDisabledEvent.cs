using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanyDisabledEvent : BaseEvent
{
    public Company Company { get; }

    public CompanyDisabledEvent(Company company)
    {
        Company = company;
    }
}
