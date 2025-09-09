using TransactR;

namespace SagaR.TransactR.Abstractions;

public interface ITransactionalSaga<TContext> : ISaga<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    string TransactionId { get; }
}
