using GoratLoans.Exceptions;

namespace GoratLoans.Loans;

public record CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());

    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new GoratLoansException($"Invalid customer id: '{value}");
        }
        
        return new CustomerId(value);
    }
}