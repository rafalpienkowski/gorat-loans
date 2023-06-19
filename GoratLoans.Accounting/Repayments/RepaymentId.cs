namespace GoratLoans.Accounting.Repayments;

public record RepaymentId(Guid Value)
{
    public static RepaymentId New() => new(Guid.NewGuid());
}