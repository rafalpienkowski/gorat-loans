using GoratLoans.Domain.Framework;

namespace GoratLoans.CRM.PublicEvents;

public record AccountCreated(Guid AccountId, Guid CustomerId) : PublicDomainEvent;