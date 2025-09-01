namespace SagaR
{
    /// <summary>
    /// Represents a single step within a programmatic saga, including its execution and compensation logic.
    /// </summary>
    /// <typeparam name="TContext">The type of the transaction context.</typeparam>
    public interface ISagaStep<TContext>
    {
        /// <summary>
        /// The business logic to execute for this step.
        /// </summary>
        /// <param name="context">The current transaction context.</param>
        Task ExecuteAsync(TContext context);

        /// <summary>
        /// The compensation logic to execute if a subsequent step fails.
        /// </summary>
        /// <param name="context">The current transaction context.</param>
        Task CompensateAsync(TContext context);
    }
}
