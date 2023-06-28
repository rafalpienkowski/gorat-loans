namespace GoratLoans;

public interface IClock
{
    DateTimeOffset Now { get; }
}