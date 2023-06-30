using GoratLoans.Framework;

namespace GoratLoans.Customers.PublicEvents;

public record AccountCreated(Guid AccountId, Guid CustomerId) : PublicDomainEvent;