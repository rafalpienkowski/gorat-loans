namespace GoratLoans.Loans;

public interface IClock
{
    DateTimeOffset Now { get; }
}