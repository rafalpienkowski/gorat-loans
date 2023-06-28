using GoratLoans.Accounting.Repayments;
using GoratLoans.Tests.Framework;

namespace GoratLoans.Accounting.Tests.Loans;

public class RepaymentShould
{
    private readonly Money _loanCapital = Money.From(1000);
    private readonly TestClock _clock = new();

    [Fact]
    public void Be_Started()
    {
        var repayment = StartLoanRepayment();

        AssertRepaymentStartedEvent(repayment);
    }

    [Fact]
    public void Calculate_Interest_After_Repayment_Period_Is_Closed()
    {
        var repayment = StartLoanRepayment();

        _clock.AddDays(Repayment.LoanPeriod);

        repayment.RecalculateInterestForDate(_clock.Now);
        
        AssertInterestRecalculatedEvent(repayment, 1);
    }

    [Fact]
    public void Do_Not_Add_Interest_When_Loan_Period_Is_Not_Finished()
    {
        var repayment = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod - 1);

        repayment.RecalculateInterestForDate(_clock.Now);
        
        var interestRecalculated = repayment.GetProducedEventByType<InterestRecalculated>();
        interestRecalculated.Should().BeNull();
    }

    [Fact]
    public void Do_Not_Add_Interest_Twice_For_A_Period()
    {
        var repayment = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod);
        repayment.RecalculateInterestForDate(_clock.Now);
        AssertInterestRecalculatedEvent(repayment, 1);
        repayment.FlushChanges();

        repayment.RecalculateInterestForDate(_clock.Now);
        
        var interestRecalculated = repayment.GetProducedEventByType<InterestRecalculated>();
        interestRecalculated.Should().BeNull();
    }

    [Fact]
    public void Repay_Loan_Should_Repay_Interest_First()
    {
        var repayment = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod * 100);
        repayment.RecalculateInterestForDate(_clock.Now);
        AssertInterestRecalculatedEvent(repayment, 100);
        repayment.FlushChanges();
        
        var repayMoney = Money.From(90);
        repayment.Repay(repayMoney);
        
        AssertRepaymentMadeEvent(repayment, 0, 90);
    }

    [Fact]
    public void Repay_Loan_Should_Repay_Capital_When_Interest_Are_Repaid()
    {
        var repayment = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod * 100);
        
        repayment.RecalculateInterestForDate(_clock.Now);
        AssertInterestRecalculatedEvent(repayment, 100);
        var repayMoney = Money.From(300);

        repayment.Repay(repayMoney);
        AssertRepaymentMadeEvent(repayment, 200, 100);
    }

    [Fact]
    public void Close_Loan_When_It_Is_Fully_Repaid()
    {
        var loan = StartLoanRepayment();
        
        loan.Repay(_loanCapital);

        AssertRepaymentIsClosedEvent(loan);
    }

    [Fact]
    public void Reject_Repayment_For_Closed_Loan()
    {
        var loan = StartLoanRepayment();
        loan.Repay(_loanCapital);
        AssertRepaymentIsClosedEvent(loan);
        loan.FlushChanges();
        
        loan.Repay(Money.From(1));
        var repaymentMade = loan.GetProducedEventByType<RepaymentMade>();
        repaymentMade.Should().BeNull();
    }

    [Fact]
    public void Reject_Repay_Over_Capital()
    {
        var repayment = StartLoanRepayment();
        var overCapitalRepayment = _loanCapital + Money.From(1);

        repayment.Repay(overCapitalRepayment);

        AssertRepaymentMadeEvent(repayment, _loanCapital.Value, 0);

        var overPaymentMade = repayment.GetProducedEventByType<OverPaymentMade>();
        overPaymentMade.Should().NotBeNull();
        overPaymentMade!.RepaymentId.Should().Be(repayment.Id);
        overPaymentMade.OverPaymentAmount.Should().Be(1);
        overPaymentMade.RepaymentCurrency.Should().Be(_loanCapital.Currency);
    }

    private void AssertRepaymentMadeEvent(Repayment repayment, decimal expectedCapitalRepaid, decimal expectedInterestRepaid)
    {
        var repaymentMade = repayment.GetProducedEventByType<RepaymentMade>();
        repaymentMade.Should().NotBeNull();
        repaymentMade!.RepaymentId.Should().Be(repayment.Id);
        repaymentMade.CapitalRepaidAmount.Should().Be(expectedCapitalRepaid);
        repaymentMade.InterestRepaidAmount.Should().Be(expectedInterestRepaid);
        repaymentMade.RepaymentCurrency.Should().Be(_loanCapital.Currency);
    }

    private void AssertInterestRecalculatedEvent(Repayment repayment, decimal expectedAmount)
    {
        var interestRecalculated = repayment.GetProducedEventByType<InterestRecalculated>();
        
        interestRecalculated.Should().NotBeNull();
        interestRecalculated!.CalculatedAt.Should().Be(_clock.Now);
        interestRecalculated.RepaymentCurrency.Should().Be(_loanCapital.Currency);
        interestRecalculated.InterestAmount.Should().Be(expectedAmount);
    }
    
    private void AssertRepaymentStartedEvent(Repayment repayment)
    {
        var repaymentStarted = repayment.GetProducedEventByType<RepaymentStarted>();
        repaymentStarted.Should().NotBeNull();
        repaymentStarted!.CapitalAmount.Should().Be(_loanCapital.Value);
        repaymentStarted.RepaymentCurrency.Should().Be(_loanCapital.Currency);
        repaymentStarted.StartedAt.Should().Be(_clock.Now);
        repaymentStarted.RepaymentId.Should().NotBeEmpty();
        repaymentStarted.LoanId.Should().NotBeEmpty();
        repaymentStarted.CustomerId.Should().NotBeEmpty();
    }
    
    private static void AssertRepaymentIsClosedEvent(Repayment loan)
    {
        var repaymentClosed = loan.GetProducedEventByType<RepaymentClosed>();
        repaymentClosed.Should().NotBeNull();
        repaymentClosed!.RepaymentId.Should().Be(loan.Id);
    }

    private Repayment StartLoanRepayment() =>
        Repayment.Start(Guid.NewGuid(), Guid.NewGuid(), _loanCapital.Value, _loanCapital.Currency, _clock);
}