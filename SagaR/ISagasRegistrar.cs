using Microsoft.Extensions.DependencyInjection;

namespace SagaR;

/// <summary>
/// Provides the root of the fluent API for registering multiple sagas.
/// </summary>
public interface ISagasRegistrar
{
    /// <summary>
    /// Starts the configuration for a specific saga identified by its context type.
    /// </summary>
    /// <typeparam name="TContext">The type of the transaction context for the saga.</typeparam>
    /// <returns>A configurator for the specific saga.</returns>
    ISagaMapConfigurator<TContext, TSaga> MapSaga<TContext, TSaga>()
        where TContext : class, new()
        where TSaga : class, ISaga<TContext>;

    /// <summary>
    /// Instructs and uses a custom request sender for dispatching saga step requests.
    /// </summary>
    /// <typeparam name="TRequestSender">The type of the request sender.</typeparam>
    /// <returns>The configurator instance for chaining more steps.</returns>
    ISagasRegistrar UseStepRequestSender<TRequestSender>()
        where TRequestSender : class, ISagaStepRequestSender;
}
