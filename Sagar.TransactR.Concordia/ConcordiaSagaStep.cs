using System;
using System.Threading.Tasks;
using Concordia;
using SagaR.TransactR.Abstractions;
using TransactR;

namespace SagaR.TransactR.Concordia;


public interface ISagaTransactiveRequest<TContext, TResponse> : IRequest<TResponse>, ITransactionalSagaStepRequest<TContext, TResponse>
    where TContext : class, ITransactionContext<string, TContext>, new()
{

}

/// <summary>
/// An abstract base class for a saga step that is also a Concordia command.
/// The business logic for this step should be implemented in a corresponding IRequestHandler.
/// </summary>
/// <typeparam name="TContext">The type of the saga's context.</typeparam>
/// <typeparam name="TState">The type of the saga's persistent state.</typeparam>
public abstract class ConcordiaSagaStep<TContext, TRequest, TResponse> : TransactionalSagaStep<TContext>, ITransactionalSagaStep<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
    where TRequest : ISagaTransactiveRequest<TContext, TResponse>
{
    public override Type RequestType => typeof(TRequest);

    protected ConcordiaSagaStep(string identifier, RollbackPolicy rollbackPolicy = RollbackPolicy.RollbackToCurrentStep) : base(identifier, rollbackPolicy)
    {
    }
}

