using GoratLoans.Framework;

namespace GoratLoans.Accounting.Offering;

public record LoanGranted : PublicDomainEvent
{
    public Guid CustomerId { get; init; }
    public Guid LoanId { get; init; }
    public decimal CapitalAmount { get; init; }
    public string CapitalCurrency { get; init; } = null!;
}
