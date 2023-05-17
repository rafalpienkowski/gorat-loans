namespace GoratLoans.Loans;

public readonly record struct Money(decimal Value, string Currency = "USD")
{
    public static Money Zero => new(0);

    public static Money From(decimal value) => new(value);

    public static bool operator >(Money a, Money b) => a.Value > b.Value;
    public static bool operator <(Money a, Money b) => a.Value < b.Value;

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new ArgumentOutOfRangeException(nameof(a), "Currency mismatch");
        }

        return new Money(a.Value + b.Value);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new ArgumentOutOfRangeException(nameof(a), "Currency mismatch");
        }

        return new Money(a.Value - b.Value);
    }

    public static Money operator *(Money a, int multiplication) => new(a.Value * multiplication);

    public override string ToString() => $"{Value:F2} {Currency}";
}