using TransactR;

namespace SagaR.TransactR.Abstractions;

public interface ITransactionalSagaStepRequest<TContext> : ISagaStepRequest, ITransactionalRequest<string, TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
}

public interface ITransactionalSagaStepRequest<TContext, TResult> : ITransactionalSagaStepRequest<TContext>, ISagaStepRequest<TResult>
    where TContext : class, ITransactionContext<string, TContext>, new()

{
}
