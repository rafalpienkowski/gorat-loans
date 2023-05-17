using GoratLoans.Loans;
using NodaTime;

namespace GoratLoans.Tests.Loans;

public class RepaymentShould
{
    private readonly Money _loanCapital = Money.From(1000);

    [Fact]
    public void Start_Granted_Loan()
    {
        var approvalDate = UtcNowToLocalDate();
        var repayment = StartLoanRepayment(approvalDate);

        repayment.Should().NotBeNull();
        repayment.Capital.Should().Be(_loanCapital);
        repayment.StartedAt.Should().Be(approvalDate);
        repayment.Interest.Should().Be(Money.Zero);
        repayment.Id.Should().NotBeNull();
        repayment.LoanId.Should().NotBeNull();
        repayment.CustomerId.Should().NotBeNull();
        repayment.LastLoanBalanceCalculatedAt.Should().Be(approvalDate);
        repayment.IsFullyRepaid.Should().BeFalse();
    }

    [Fact]
    public void Calculate_Interest_After_Repayment_Period()
    {
        var repayment = StartLoanRepayment();

        var now = UtcNowToLocalDate();
        var dayAfterRepaymentPeriod = now + repayment.LoanPeriod + Period.FromDays(1);

        repayment.RecalculateLoanAt(dayAfterRepaymentPeriod);

        repayment.Interest.ToString().Should().Be(Money.From(8.33m).ToString());
    }

    [Fact]
    public void Do_Not_Add_Interest_When_Loan_Period_Is_Not_Finished()
    {
        var repayment = StartLoanRepayment();

        var now = UtcNowToLocalDate();
        var dayBeforeRepaymentPeriod = now + repayment.LoanPeriod + Period.FromDays(-1);

        repayment.RecalculateLoanAt(dayBeforeRepaymentPeriod);
        repayment.Interest.ToString().Should().Be(Money.Zero.ToString());
    }

    [Fact]
    public void Do_Not_Add_Interest_Twice_For_A_Period()
    {
        var repayment = StartLoanRepayment();

        var now = UtcNowToLocalDate();
        var dayAfterRepaymentPeriod = now + repayment.LoanPeriod + Period.FromDays(1);

        repayment.RecalculateLoanAt(dayAfterRepaymentPeriod);
        repayment.Interest.ToString().Should().Be(Money.From(8.33m).ToString());

        repayment.RecalculateLoanAt(dayAfterRepaymentPeriod);
        repayment.Interest.ToString().Should().Be(Money.From(8.33m).ToString());
    }

    [Fact]
    public void Repay_Loan_Should_Repay_Interest_First()
    {
        var loan = StartLoanRepayment();
        var now = UtcNowToLocalDate();
        var quarterAfterLoanIsStarted = now + loan.LoanPeriod + loan.LoanPeriod + loan.LoanPeriod;
        
        loan.RecalculateLoanAt(quarterAfterLoanIsStarted);
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
        var now = UtcNowToLocalDate();
        var quarterAfterLoanIsStarted = now + loan.LoanPeriod + loan.LoanPeriod + loan.LoanPeriod;
        
        loan.RecalculateLoanAt(quarterAfterLoanIsStarted);
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
        loan.TotalAmountToPay.ToString().Should().Be(_loanCapital.ToString());
        
        
        var now = UtcNowToLocalDate();
        var quarterAfterLoanIsStarted = now + loan.LoanPeriod + loan.LoanPeriod + loan.LoanPeriod;
        loan.RecalculateLoanAt(quarterAfterLoanIsStarted);
        
        loan.TotalAmountToPay.ToString().Should().Be(Money.From(1016.67m).ToString());
    }

    private Repayment StartLoanRepayment() => StartLoanRepayment(UtcNowToLocalDate());
    
    private Repayment StartLoanRepayment(LocalDate approvalDate)
    {
        var application = new LoanApplication(CustomerId.New(), _loanCapital);
        return application.Approve(approvalDate);
    }

    private static LocalDate UtcNowToLocalDate()
    {
        var now = DateTime.UtcNow;
        return new LocalDate(now.Year, now.Month, now.Day);
    }
}