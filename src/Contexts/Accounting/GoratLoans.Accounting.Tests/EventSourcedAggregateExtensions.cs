using GoratLoans.Domain.Framework;

namespace GoratLoans.Accounting.Tests;

public static class EventSourcedAggregateExtensions
{
    public static T? GetProducedEventByType<T>(this EventSourcedAggregate aggregate) where T : DomainEvent =>
        (T)aggregate.Changes.SingleOrDefault(change => change.GetType() == typeof(T))!;
}