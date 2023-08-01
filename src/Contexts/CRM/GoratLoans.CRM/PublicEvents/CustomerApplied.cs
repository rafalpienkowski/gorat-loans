using GoratLoans.Domain.Framework;

namespace GoratLoans.CRM.PublicEvents;

public record CustomerApplied
    (Guid CustomerId, string FirstName, string LastName, DateOnly BirthDate, string Address) : PublicDomainEvent;