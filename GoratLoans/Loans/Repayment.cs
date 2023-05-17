using NodaTime;

namespace GoratLoans.Loans;

public class Repayment
{
    private readonly InterestRate _yearlyInterestRate = new(0.1);
    private const int LoanDuration = 12;
    public readonly Period LoanPeriod = Period.FromDays(30);

    public RepaymentId Id { get; }
    public LoanId LoanId { get; }
    public CustomerId CustomerId { get; }
    public Money Interest { get; private set; } = new(0m);
    public Money Capital { get; private set; }
    public Money TotalAmountToPay => Interest + Capital;
    public bool IsFullyRepaid => TotalAmountToPay == Money.Zero;
    public LocalDate StartedAt { get; }
    public LocalDate LastLoanBalanceCalculatedAt { get; private set; }

    private Repayment(RepaymentId repaymentId, LoanId loanId, CustomerId customerId, Money capital, LocalDate startedAt,
        LocalDate lastLoanBalanceCalculatedAt)
    {
        Id = repaymentId;
        LoanId = loanId;
        CustomerId = customerId;
        Capital = capital;
        StartedAt = startedAt;
        LastLoanBalanceCalculatedAt = lastLoanBalanceCalculatedAt;
    }

    public static Repayment StartWith(CustomerId customerId, LoanId loanId, Money capital, LocalDate now)
    {
        var loanRepayment = new Repayment(RepaymentId.New(), loanId, customerId, capital, now, now);

        return loanRepayment;
    }

    public void RecalculateLoanAt(LocalDate date)
    {
        var periodBetweenLastCalculation = Period.Between(LastLoanBalanceCalculatedAt, date, PeriodUnits.Months);
        var monthsBetween = periodBetweenLastCalculation.Months;

        var interest = (double)(Capital.Value + Interest.Value) * (_yearlyInterestRate.Value / LoanDuration) *
                       monthsBetween;

        Interest += new Money((decimal)interest);
        LastLoanBalanceCalculatedAt = date;
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