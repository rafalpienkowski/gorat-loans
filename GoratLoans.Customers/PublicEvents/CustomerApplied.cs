using GoratLoans.Framework;

namespace GoratLoans.Customers.PublicEvents;

public record CustomerApplied
    (Guid CustomerId, string FirstName, string LastName, DateOnly BirthDate, string Address) : PublicDomainEvent;