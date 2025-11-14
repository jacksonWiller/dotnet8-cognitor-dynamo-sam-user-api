using Arda9UserApi.Core;
using Catalog.Domain.Entities.UserAggregate;

namespace Arda9UserApi.Domain.Entities.UserAggregate.Events;

public class UserUpdatedEvent : BaseEvent
{
    public User User { get; }

    public UserUpdatedEvent(User user)
    {
        User = user;
    }
}