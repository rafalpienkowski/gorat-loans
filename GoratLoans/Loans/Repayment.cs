namespace GoratLoans.Loans;

public class Repayment
{
    private readonly InterestRate _yearlyInterestRate = new(0.365);
    private const int LoanDuration = 365;
    public const int LoanPeriod = 1;
    private readonly IClock _clock;

    public RepaymentId Id { get; }
    public LoanId LoanId { get; }
    public CustomerId CustomerId { get; }
    public Money Interest { get; private set; } = new(0m);
    public Money Capital { get; private set; }
    public Money TotalAmountToPay => Interest + Capital;
    public bool IsFullyRepaid => TotalAmountToPay == Money.Zero;
    public DateTimeOffset StartedAt { get; }
    public DateTimeOffset LastLoanBalanceCalculatedAt { get; private set; }

    private Repayment(RepaymentId repaymentId, LoanId loanId, CustomerId customerId, Money capital, DateTimeOffset startedAt,
        DateTimeOffset lastLoanBalanceCalculatedAt, IClock clock)
    {
        Id = repaymentId;
        LoanId = loanId;
        CustomerId = customerId;
        Capital = capital;
        StartedAt = startedAt;
        LastLoanBalanceCalculatedAt = lastLoanBalanceCalculatedAt;
        _clock = clock;
    }

    public static Repayment StartWith(CustomerId customerId, LoanId loanId, Money capital, IClock clock)
    {
        var now = clock.Now;
        var loanRepayment = new Repayment(RepaymentId.New(), loanId, customerId, capital, now, now, clock);

        return loanRepayment;
    }

    public void Recalculate()
    {
        var now = _clock.Now;
        var daysBetweenLastCalculation = Math.Floor((now - LastLoanBalanceCalculatedAt).TotalDays);

        var interest = (double)(Capital.Value + Interest.Value) * (_yearlyInterestRate.Value / LoanDuration) *
                       daysBetweenLastCalculation;

        Interest += new Money((decimal)interest);
        LastLoanBalanceCalculatedAt = now;
    }

    public void Repay(Money money)
    {
        if (IsFullyRepaid)
        {
            throw new InvalidOperationException("Loan is fully repaid");
        }

        var overInterest = money - Interest;
        if (overInterest > Capital)
        {
            throw new InvalidOperationException("Can't accept repayment greater than remaining capital");
        }

        if (overInterest > Money.Zero)
        {
            Capital -= overInterest;
            Interest = Money.Zero;
        }
        else
        {
            Interest -= money;
        }
    }
}