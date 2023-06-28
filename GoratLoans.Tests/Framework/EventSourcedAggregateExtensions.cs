using GoratLoans.Framework;

namespace GoratLoans.Tests.Framework;

public static class EventSourcedAggregateExtensions
{
    public static T? GetProducedEventByType<T>(this EventSourcedAggregate aggregate) where T : DomainEvent =>
        (T)aggregate.Changes.SingleOrDefault(change => change.GetType() == typeof(T)) ?? default(T);
}