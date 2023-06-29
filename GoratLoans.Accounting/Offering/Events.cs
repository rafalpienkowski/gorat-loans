using GoratLoans.Framework;

namespace GoratLoans.Accounting.Offering;

public record LoanApplicationStarted(Guid LoanApplicationId, Guid CustomerId, decimal CapitalAmount,
    string CapitalCurrency) : DomainEvent;

public record LoanGranted(Guid LoanApplicationId, Guid LoanId) : DomainEvent;

public record LoanApplicationRejected(Guid LoanApplicationId, string Reason) : DomainEvent;