using GoratLoans.Exceptions;

namespace GoratLoans.Accounting.Offering;

public record LoanId(Guid Value)
{
    public static LoanId New() => new(Guid.NewGuid());

    public static LoanId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new GoratLoansException($"Invalid loan id: '{value}'");
        }

        return new LoanId(value);
    }
}