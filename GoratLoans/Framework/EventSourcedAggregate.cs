namespace GoratLoans.Framework;

public abstract class EventSourcedAggregate : Entity
{
    public List<DomainEvent> Changes { get; private set; } = new();
    public int Version { get; protected set; } = 0;

    public abstract void Apply(DomainEvent change);

    protected void Causes(DomainEvent @event)
    {
        Changes.Add(@event);
        Apply(@event);
    }

    public void FlushChanges() => Changes = new List<DomainEvent>();
}