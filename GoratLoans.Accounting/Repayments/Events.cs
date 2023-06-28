using GoratLoans.Framework;

namespace GoratLoans.Accounting.Repayments;

public record RepaymentStarted(Guid RepaymentId, Guid LoanId, Guid CustomerId, decimal CapitalAmount,
    string RepaymentCurrency, DateTimeOffset StartedAt) : DomainEvent;

public record InterestRecalculated(Guid RepaymentId, decimal InterestAmount, string RepaymentCurrency,
    DateTimeOffset CalculatedAt) : DomainEvent;

public record RepaymentMade
(Guid RepaymentId, decimal CapitalRepaidAmount, decimal InterestRepaidAmount,
    string RepaymentCurrency) : DomainEvent;

public record OverPaymentMade(Guid RepaymentId, decimal OverPaymentAmount, string RepaymentCurrency) : DomainEvent;

public record RepaymentClosed(Guid RepaymentId) : DomainEvent;