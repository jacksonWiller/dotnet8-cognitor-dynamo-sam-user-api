using Arda9UserApi.Core;

namespace Catalog.Domain.Entities.CompanyAggregate.Events;

public class CompanySuspendedEvent : BaseEvent
{
    public Company Company { get; }
    public string Reason { get; }

    public CompanySuspendedEvent(Company company, string reason)
    {
        Company = company;
        Reason = reason;
    }
}
