using GoratLoans.Framework;

namespace GoratLoans.Customers.PublicEvents;

public record CustomerVerified(Guid CustomerId) : PublicDomainEvent;