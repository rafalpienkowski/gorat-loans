using GoratLoans.Accounting.Offering;
using GoratLoans.Tests.Framework;

namespace GoratLoans.Accounting.Tests.Offering;

public class LoanApplicationShould
{
    private readonly CustomerId _raf = CustomerId.From("06d3e8ff-9d8f-4ee0-b4c9-3a32209a92dc");
    private readonly Guid _loanApplicationId = Guid.NewGuid();

    [Fact]
    public void Be_Started()
    {
        var customerId = CustomerId.New();
        var capitalMoney = Money.From(2000);
        var loanApplication = StartLoanApplicationFor(customerId, capitalMoney);

        AssertLoanApplicationStartedEvent(loanApplication, customerId, capitalMoney);
    }

    [Fact]
    public void Be_Rejected_For_Small_Loans()
    {
        var customerId = CustomerId.New();
        var capitalMoney = Money.From(20);
        var loanApplication = StartLoanApplicationFor(customerId, capitalMoney);

        loanApplication.Judge();

        AssertLoanApplicationRejectedEvent(loanApplication,
            "Only loans with capital bigger than 100,00 USD are accepted");
    }

    [Fact]
    public void Be_Granted_For_Big_Loans()
    {
        var customerId = CustomerId.New();
        var capitalMoney = Money.From(20000);
        var loanApplication = StartLoanApplicationFor(customerId, capitalMoney);

        loanApplication.Judge();

        AssertLoanGrantedEvent(loanApplication);
    }

    [Fact]
    public void Be_Rejected_For_Raf_Even_If_Loan_Is_Big()
    {
        var capitalMoney = Money.From(20000);
        var loanApplication = StartLoanApplicationFor(_raf, capitalMoney);

        loanApplication.Judge();

        AssertLoanApplicationRejectedEvent(loanApplication, "Raf can't effort a loan");
    }

    [Fact]
    public void Not_Be_Judged_Twice()
    {
        var customerId = CustomerId.New();
        var capitalMoney = Money.From(20000);
        var loanApplication = StartLoanApplicationFor(customerId, capitalMoney);
        loanApplication.Judge();
        AssertLoanGrantedEvent(loanApplication);
        loanApplication.FlushChanges();
        
        loanApplication.Judge();
        loanApplication.Changes.Should().BeEmpty();
    }
    
    private LoanApplication StartLoanApplicationFor(CustomerId customerId, Money capitalMoney)
    {
        var loanApplication = LoanApplication.Start(_loanApplicationId, customerId, capitalMoney);
        return loanApplication;
    }

    private void AssertLoanApplicationStartedEvent(LoanApplication loanApplication, CustomerId customerId,
        Money capitalMoney)
    {
        var loanApplicationStated = loanApplication.GetProducedEventByType<LoanApplicationStarted>();
        loanApplicationStated.Should().NotBeNull();
        loanApplicationStated!.CustomerId.Should().Be(customerId.Value);
        loanApplicationStated.LoanApplicationId.Should().Be(_loanApplicationId);
        loanApplicationStated.CapitalAmount.Should().Be(capitalMoney.Value);
        loanApplicationStated.CapitalCurrency.Should().Be(capitalMoney.Currency);
    }

    private void AssertLoanApplicationRejectedEvent(LoanApplication loanApplication, string reason)
    {
        var loanApplicationRejected = loanApplication.GetProducedEventByType<LoanApplicationRejected>();
        loanApplicationRejected.Should().NotBeNull();
        loanApplicationRejected!.LoanApplicationId.Should().Be(_loanApplicationId);
        loanApplicationRejected.Reason.Should().Be(reason);
    }

    private void AssertLoanGrantedEvent(LoanApplication loanApplication)
    {
        var loanGranted = loanApplication.GetProducedEventByType<LoanGranted>();
        loanGranted.Should().NotBeNull();
        loanGranted!.LoanApplicationId.Should().Be(_loanApplicationId);
        loanGranted.LoanId.Should().NotBeEmpty();
    }
}