using GoratLoans.Framework;
using GoratLoans.Loans;

namespace GoratLoans.Accounting.Repayments;

public class Repayment : EventSourcedAggregate
{
    private readonly InterestRate _yearlyInterestRate = new(0.365);
    private const int LoanDuration = 365;
    public const int LoanPeriod = 1;

    private LoanId _loanId;
    private CustomerId _customerId;
    private Money _interest;
    private Money _capital;
    private bool _isClosed;
    private DateTimeOffset _startedAt;
    private DateTimeOffset _lastLoanBalanceCalculatedAt;

    public void RecalculateInterestForDate(DateTimeOffset dateTimeOffset)
    {
        var daysBetweenLastCalculation = Math.Floor((dateTimeOffset - _lastLoanBalanceCalculatedAt).TotalDays);
        if (daysBetweenLastCalculation <= 0)
        {
            return;
        }

        var interest = (decimal)((double)(_capital.Value + _interest.Value) * (_yearlyInterestRate.Value / LoanDuration) *
                                 daysBetweenLastCalculation);

        Causes(new InterestRecalculated(Id, interest, _capital.Currency, dateTimeOffset));
    }

    public void Repay(Money money)
    {
        if (_isClosed)
        {
            return;
        }

        var overInterest = money - _interest;
        if (overInterest > _capital)
        {
            Causes(new OverPaymentMade(Id, (money - _capital).Value, _capital.Currency));
            Causes(new RepaymentMade(Id, _capital.Value, _interest.Value, _capital.Currency));
        }
        else
        {
            Causes(overInterest > Money.Zero
                ? new RepaymentMade(Id, overInterest.Value, _interest.Value, _capital.Currency)
                : new RepaymentMade(Id, Money.Zero.Value, money.Value, _capital.Currency));
        }

        if (_capital == Money.Zero)
        {
            Causes(new RepaymentClosed(Id));
        }
    }

    public override void On(DomainEvent @event)
    {
        On((dynamic)@event);
        Version++;
    }

    private void On(RepaymentStarted repaymentStarted)
    {
        Id = repaymentStarted.RepaymentId;
        _loanId = LoanId.From(repaymentStarted.LoanId);
        _customerId = CustomerId.From(repaymentStarted.CustomerId);
        var capital = Money.From(repaymentStarted.CapitalAmount, repaymentStarted.RepaymentCurrency);
        _capital = capital;
        _interest = Money.Zero;
        _startedAt = repaymentStarted.RecorderAt;
        _lastLoanBalanceCalculatedAt = repaymentStarted.RecorderAt;
    }

    private void On(InterestRecalculated interestRecalculated)
    {
        var interest = Money.From(interestRecalculated.InterestAmount, interestRecalculated.RepaymentCurrency);

        _lastLoanBalanceCalculatedAt = interestRecalculated.CalculatedAt;
        _interest += interest;
    }

    private void On(RepaymentMade repaymentMade)
    {
        var interestRepaid = Money.From(repaymentMade.InterestRepaidAmount, repaymentMade.RepaymentCurrency);
        var capitalRepaid = Money.From(repaymentMade.CapitalRepaidAmount, repaymentMade.RepaymentCurrency);

        _interest -= interestRepaid;
        _capital -= capitalRepaid;
    }

    private void On(RepaymentClosed repaymentClosed)
    {
        _isClosed = true;
    }

    private void On(OverPaymentMade overPaymentMade)
    {
        //Store for information purposes
    }

    public static Repayment Start(Guid loanId, Guid customerId, decimal capitalAmount, string capitalCurrency,
        IClock clock)
    {
        var repayment = new Repayment();

        repayment.Causes(new RepaymentStarted(Guid.NewGuid(), loanId, customerId, capitalAmount, capitalCurrency,
            clock.Now));

        return repayment;
    }
}