namespace SagaR;


/// <summary>
/// Represents a defined programmatic saga, which is an ordered collection of saga steps.
/// </summary>
/// <typeparam name="TContext">The type of the transaction context used by this saga.</typeparam>
public abstract class Saga<TContext> : ISaga<TContext>
{
    public IEnumerable<ISagaStep<TContext>> Steps { get; }

    public Saga(IEnumerable<ISagaStep<TContext>> steps)
    {
        Steps = steps;
    }
}
