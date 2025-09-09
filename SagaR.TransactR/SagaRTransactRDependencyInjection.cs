using System;
using Microsoft.Extensions.DependencyInjection;
using SagaR.TransactR.Abstractions;
using SagaR.TransactR.Internal;
using TransactR;
using static SagaR.SagaRDependencyInjection;

namespace SagaR.TransactR;

public interface ITransactionalSagaMapConfigurator<TContext, TSaga> : ISagaMapConfigurator<TContext, TSaga>
    where TContext : class, ITransactionContext<string, TContext>, new()
    where TSaga : class, ITransactionalSaga<TContext>
{
    ITransactorBuilder<string, TContext> TransactorBuilder { get; }
}

internal abstract class TransactionalSagaMapConfigurator<TContext, TSaga> : SagaMapConfigurator<TContext, TSaga>, ITransactionalSagaMapConfigurator<TContext, TSaga>
    where TContext : class, ITransactionContext<string, TContext>, new()
    where TSaga : class, ITransactionalSaga<TContext>
{
    public ITransactorBuilder<string, TContext> TransactorBuilder { get; protected set; }

    public TransactionalSagaMapConfigurator(
        ISagaMapConfigurator<TContext, TSaga> sagaMapConfigurator,
        ITransactorBuilder<string, TContext> transactorBuilder) : base(sagaMapConfigurator)
    {
        TransactorBuilder = transactorBuilder ?? throw new ArgumentNullException(nameof(transactorBuilder));
    }
}

