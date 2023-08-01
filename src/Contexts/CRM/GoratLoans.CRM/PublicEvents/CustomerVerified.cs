using GoratLoans.Domain.Framework;

namespace GoratLoans.CRM.PublicEvents;

public record CustomerVerified(Guid CustomerId) : PublicDomainEvent;