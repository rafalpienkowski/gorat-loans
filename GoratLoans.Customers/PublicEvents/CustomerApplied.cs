using GoratLoans.Framework;

namespace GoratLoans.Customers.PublicEvents;

public record CustomerApplied
    (Guid CustomerId, string FirstName, string LastName, DateOnly BirthDate, string Address) : PublicDomainEvent;

public record CustomerVerified(Guid CustomerId) : PublicDomainEvent;

public record AccountCreated(Guid AccountId, Guid CustomerId) : PublicDomainEvent;