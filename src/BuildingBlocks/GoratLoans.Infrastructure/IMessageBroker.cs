namespace GoratLoans.Infrastructure;

public interface IMessageBroker : IDisposable
{
    void SubscribeTo<TIn, TOut>(string inQueueName, string outQueueName, Func<TIn, TOut> onMessageFunction)
        where TIn : class, new() where TOut : class, new();
}