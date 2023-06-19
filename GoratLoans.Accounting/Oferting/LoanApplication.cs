using GoratLoans.Accounting.Repayments;
using GoratLoans.Loans;

namespace GoratLoans.Accounting.Oferting;

public class LoanApplication
{
    private Money Capital { get; }
    private CustomerId CustomerId { get; }
    private readonly IClock _clock;

    public LoanApplication(CustomerId customerId, Money capital, IClock clock)
    {
        CustomerId = customerId;
        Capital = capital;
        _clock = clock;
    }

    public Repayment Approve()
    {
        return Repayment.StartWith(CustomerId, LoanId.New(), Capital, _clock);
    }
}