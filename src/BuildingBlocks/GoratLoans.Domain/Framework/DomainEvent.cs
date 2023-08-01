namespace GoratLoans.Domain.Framework;

public abstract record DomainEvent
{
    public DateTimeOffset RecorderAt { get; init; } = DateTimeOffset.UtcNow;
    public int Version { get; init; }
}