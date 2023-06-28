using GoratLoans.Accounting.Repayments;
using GoratLoans.Framework;
using GoratLoans.Loans;

namespace GoratLoans.Accounting.Offering;

public class LoanApplication
{
    private Money Capital { get; }
    private CustomerId CustomerId { get; }

    public LoanApplication(CustomerId customerId, Money capital)
    {
        CustomerId = customerId;
        Capital = capital;
    }
}