namespace SagaR
{
    public interface ISaga<TContext> { 
        IReadOnlyList<ISagaStep<TContext>> Steps { get; }
    }
    /// <summary>
    /// Represents a defined programmatic saga, which is an ordered collection of saga steps.
    /// </summary>
    /// <typeparam name="TContext">The type of the transaction context used by this saga.</typeparam>
    public abstract class Saga<TContext> : ISaga<TContext>
    {
        public IReadOnlyList<ISagaStep<TContext>> Steps { get; }

        public Saga(IEnumerable<ISagaStep<TContext>> steps)
        {
            Steps = steps.ToList().AsReadOnly();
        }
    }
}
