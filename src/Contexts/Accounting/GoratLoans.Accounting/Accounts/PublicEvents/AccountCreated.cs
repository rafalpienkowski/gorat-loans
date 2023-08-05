using GoratLoans.Domain.Framework;

namespace GoratLoans.Accounting.Accounts.PublicEvents;

public record AccountCreated(Guid AccountId, Guid CustomerId) : PublicDomainEvent;