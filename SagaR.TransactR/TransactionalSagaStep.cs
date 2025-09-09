using SagaR.TransactR.Abstractions;
using TransactR;

namespace SagaR.TransactR;

public abstract class TransactionalSagaStep<TContext> : ITransactionalSagaStep<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    public string TransactionId { get; private set; } = string.Empty;

    public IComparable Step => Identifier;

    public RollbackPolicy RollbackPolicy { get; private set; }

    public string Identifier { get; private set; }

    public abstract Type RequestType { get; }

    public TransactionalSagaStep(string identifier, RollbackPolicy rollbackPolicy = RollbackPolicy.RollbackToCurrentStep)
    {
        Identifier = identifier;
        RollbackPolicy = rollbackPolicy;
    }

    public void WithTransactionId(string transactionId)
    {
        TransactionId = transactionId;
    }

    public abstract ISagaStepRequest RequestFactory(TContext context);

    public abstract Task CompensateAsync(TContext context, CancellationToken cancellationToken = default);
}
