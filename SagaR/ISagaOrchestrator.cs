namespace SagaR;

/// <summary>
/// Defines the contract for a saga orchestrator, which is responsible for executing
/// a saga's steps and coordinating compensation logic.
/// </summary>
/// <typeparam name="TContext">The type of the transaction context.</typeparam>
public interface ISagaOrchestrator<TContext>
{
    /// <summary>
    /// Runs the specified saga using the provided context.
    /// </summary>
    /// <param name="saga">The saga definition to execute.</param>
    /// <param name="context">The transaction context for the saga execution.</param>
    Task RunAsync(ISaga<TContext> saga, TContext context, CancellationToken cancellationToken = default);
}

