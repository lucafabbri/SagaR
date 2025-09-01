namespace SagaR
{
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
            where TSaga : Saga<TContext>, ISaga<TContext>;
    }
}
