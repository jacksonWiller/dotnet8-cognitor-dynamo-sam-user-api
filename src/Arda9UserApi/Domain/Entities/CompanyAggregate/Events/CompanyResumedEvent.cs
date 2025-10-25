using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanyResumedEvent : BaseEvent
{
    public Company Company { get; }

    public CompanyResumedEvent(Company company)
    {
        Company = company;
    }
}
