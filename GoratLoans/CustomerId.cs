using GoratLoans.Exceptions;

namespace GoratLoans;

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

    public static CustomerId From(string value) => From(new Guid(value));
}