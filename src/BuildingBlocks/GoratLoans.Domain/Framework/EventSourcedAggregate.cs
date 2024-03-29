namespace GoratLoans.Domain.Framework;

public abstract class EventSourcedAggregate : Entity
{
    public List<DomainEvent> Changes { get; private set; } = new();
    public int Version { get; protected set; } = 0;

    public abstract void On(DomainEvent @event);

    protected void Causes(DomainEvent @event)
    {
        if (Version > @event.Version)
        {
            return;
        }
        Changes.Add(@event);
        On(@event);
    }
    
    public void FlushChanges() => Changes = new List<DomainEvent>();
}