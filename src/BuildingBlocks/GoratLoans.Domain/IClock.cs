namespace GoratLoans.Domain;

public interface IClock
{
    DateTimeOffset Now { get; }
}