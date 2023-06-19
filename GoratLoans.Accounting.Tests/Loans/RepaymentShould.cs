using GoratLoans.Accounting.Oferting;
using GoratLoans.Accounting.Repayments;
using GoratLoans.Loans;

namespace GoratLoans.Accounting.Tests.Loans;

public class RepaymentShould
{
    private readonly Money _loanCapital = Money.From(1000);
    private readonly TestClock _clock = new();

    [Fact]
    public void Start_Granted_Loan()
    {
        var repayment = StartLoanRepayment();

        repayment.Should().NotBeNull();
        repayment.Capital.Should().Be(_loanCapital);
        repayment.StartedAt.Should().Be(_clock.Now);
        repayment.Interest.Should().Be(Money.Zero);
        repayment.Id.Should().NotBeNull();
        repayment.LoanId.Should().NotBeNull();
        repayment.CustomerId.Should().NotBeNull();
        repayment.LastLoanBalanceCalculatedAt.Should().Be(_clock.Now);
        repayment.IsFullyRepaid.Should().BeFalse();
    }

    [Fact]
    public void Calculate_Interest_After_Repayment_Period()
    {
        var repayment = StartLoanRepayment();

        _clock.AddDays(Repayment.LoanPeriod);

        repayment.Recalculate();

        repayment.Interest.Should().Be(Money.From(1m));
    }

    [Fact]
    public void Do_Not_Add_Interest_When_Loan_Period_Is_Not_Finished()
    {
        var repayment = StartLoanRepayment();

        _clock.AddDays(Repayment.LoanPeriod - 1);

        repayment.Recalculate();
        repayment.Interest.Should().Be(Money.Zero);
    }

    [Fact]
    public void Do_Not_Add_Interest_Twice_For_A_Period()
    {
        var repayment = StartLoanRepayment();

        _clock.AddDays(Repayment.LoanPeriod);

        repayment.Recalculate();
        repayment.Interest.Should().Be(Money.From(1m));

        repayment.Recalculate();
        repayment.Interest.Should().Be(Money.From(1m));
    }

    [Fact]
    public void Repay_Loan_Should_Repay_Interest_First()
    {
        var loan = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod * 100);
        
        loan.Recalculate();
        var interest = loan.Interest;
        interest.Value.Should().BePositive();

        var remainingValue = Money.From(1);
        var repayMoney = interest - remainingValue;
        repayMoney.Value.Should().BePositive();

        loan.Repay(repayMoney);
        loan.Interest.Value.Should().BePositive();
        loan.Interest.Should().Be(remainingValue);
    }

    [Fact]
    public void Repay_Loan_Should_Repay_Capital_When_Interest_Are_Repaid()
    {
        var loan = StartLoanRepayment();
        _clock.AddDays(Repayment.LoanPeriod * 100);
        
        loan.Recalculate();
        var interest = loan.Interest;
        interest.Value.Should().BePositive();
        var repayMoney = interest * 3;
        var expectedCapital = loan.Capital - (interest * 2);
        repayMoney.Value.Should().BePositive();

        loan.Repay(repayMoney);
        loan.Interest.Should().Be(Money.Zero);
        loan.Capital.Should().Be(expectedCapital);
    }

    [Fact]
    public void Close_Loan_When_It_Is_Fully_Repaid()
    {
        var loan = StartLoanRepayment();
        
        loan.Repay(loan.Capital);

        loan.IsFullyRepaid.Should().BeTrue();
    }

    [Fact]
    public void Reject_Repayment_For_Closed_Loan()
    {
        var loan = StartLoanRepayment();
        loan.Repay(loan.Capital);
        loan.IsFullyRepaid.Should().BeTrue();

        var repay = () => loan.Repay(Money.From(1));

        repay.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reject_Repay_Over_Capital()
    {
        var loan = StartLoanRepayment();
        var overCapitalRepayment = loan.Capital + Money.From(1);

        var repay = () => loan.Repay(overCapitalRepayment);

        repay.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Provide_Total_Amount_To_Pay()
    {
        var loan = StartLoanRepayment();
        loan.TotalAmountToPay.Should().Be(_loanCapital);
        _clock.AddDays(Repayment.LoanPeriod * 100);
        
        loan.Recalculate();
        
        loan.TotalAmountToPay.Should().Be(Money.From(1100m));
    }

    private Repayment StartLoanRepayment()
    {
        var application = new LoanApplication(CustomerId.New(), _loanCapital, _clock);
        return application.Approve();
    }
}