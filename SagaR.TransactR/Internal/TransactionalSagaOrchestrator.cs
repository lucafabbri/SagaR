using Microsoft.Extensions.Logging;
using TransactR;

namespace SagaR.TransactR.Internal;

/// <summary>
/// An orchestrator for transactional sagas that enforces the context
/// to be compatible with TransactR. It relies on the base implementation
/// to execute the saga steps sequentially.
/// </summary>
internal class TransactionalSagaOrchestrator<TContext> : SagaOrchestrator<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    public TransactionalSagaOrchestrator(ISagaStepRequestSender sender, ILogger<SagaOrchestrator<TContext>> logger) : base(sender, logger)
    {
    }
}

