using NodaTime;

namespace GoratLoans.Loans;

public class LoanApplication
{
    private Money Capital { get; }
    private CustomerId CustomerId { get; }

    public LoanApplication(CustomerId customerId, Money capital)
    {
        CustomerId = customerId;
        Capital = capital;
    }

    public Repayment Approve(LocalDate now)
    {
        return Repayment.StartWith(CustomerId, LoanId.New(), Capital, now);
    }
}