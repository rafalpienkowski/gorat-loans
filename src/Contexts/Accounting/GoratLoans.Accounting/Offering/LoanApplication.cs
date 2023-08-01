using GoratLoans.Domain;
using GoratLoans.Domain.Framework;

namespace GoratLoans.Accounting.Offering;

public class LoanApplication : EventSourcedAggregate
{
    private Money _capital;
    private CustomerId _customerId;
    private Money MinLoan = Money.From(100);
    private CustomerId Raf = CustomerId.From("06d3e8ff-9d8f-4ee0-b4c9-3a32209a92dc");
    private bool _isClosed;

    public void Judge()
    {
        if (_isClosed)
        {
            return;
        }
        
        if (_capital < MinLoan)
        {
            Causes(new LoanApplicationRejected(Id, $"Only loans with capital bigger than {MinLoan} are accepted")
                { Version = Version + 1 });
            return;
        }

        if (_customerId == Raf)
        {
            Causes(new LoanApplicationRejected(Id, "Raf can't effort a loan") { Version = Version + 1 });
            return;
        }

        Causes(new LoanGranted(Id, Guid.NewGuid()) { Version = Version + 1 });
    }

    public override void On(DomainEvent @event)
    {
        On((dynamic)@event);
    }

    private void On(LoanApplicationStarted loanApplicationStarted)
    {
        Id = loanApplicationStarted.LoanApplicationId;
        _capital = Money.From(loanApplicationStarted.CapitalAmount, loanApplicationStarted.CapitalCurrency);
        _customerId = CustomerId.From(loanApplicationStarted.CustomerId);
        Version = loanApplicationStarted.Version;
    }

    private void On(LoanApplicationRejected loanApplicationRejected)
    {
        _isClosed = true;
        Version = loanApplicationRejected.Version;
    }

    private void On(LoanGranted loanGranted)
    {
        _isClosed = true;
        Version = loanGranted.Version;
    }

    public static LoanApplication Start(Guid loanApplicationId, CustomerId customerId, Money capital)
    {
        var loanApplication = new LoanApplication();

        loanApplication.Causes(
            new LoanApplicationStarted(loanApplicationId, customerId.Value, capital.Value, capital.Currency)
                { Version = 0 });

        return loanApplication;
    }
}