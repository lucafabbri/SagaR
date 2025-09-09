using TransactR;

namespace SagaR.TransactR.Abstractions;


/// <summary>
/// Represents a single step within a transactional saga.
/// This marker interface combines the SagaR step definition with a specific transactional state type.
/// </summary>
/// <typeparam name="TContext">The type of the transaction context.</typeparam>
/// <typeparam name="TState">The type of the persistent state, which must be compatible with TransactR's StringState.</typeparam>
public interface ITransactionalSagaStep<TContext> : ISagaStep<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    void WithTransactionId(string transactionId);
}
