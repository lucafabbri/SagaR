using Microsoft.Extensions.DependencyInjection;

namespace SagaR
{

    /// <summary>
    /// Provides the fluent API for configuring a single saga's steps.
    /// </summary>
    /// <typeparam name="TContext">The type of the transaction context for the saga.</typeparam>
    public interface ISagaMapConfigurator<TContext, TSaga>
    {
        /// <summary>
        /// Adds a step to the current saga definition and registers its type in the DI container.
        /// </summary>
        /// <typeparam name="TStep">The type of the step, which must implement ISagaStep.</typeparam>
        /// <param name="lifetime">The service lifetime for the step registration. Defaults to Scoped.</param>
        /// <returns>The configurator instance for chaining more steps.</returns>
        ISagaMapConfigurator<TContext, TSaga> WithStep<TStep>(ServiceLifetime lifetime = ServiceLifetime.Scoped) 
            where TStep : class, ISagaStep<TContext>;

        /// <summary>
        /// Allows chaining the configuration of another saga.
        /// </summary>
        /// <returns>The root saga registrar.</returns>
        ISagasRegistrar And();
    }
}
