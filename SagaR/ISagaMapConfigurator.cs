using Microsoft.Extensions.DependencyInjection;

namespace SagaR;


/// <summary>
/// Provides the fluent API for configuring a single saga's steps.
/// </summary>
/// <typeparam name="TContext">The type of the transaction context for the saga.</typeparam>
public interface ISagaMapConfigurator<TContext, TSaga>
    where TContext : class, new()
    where TSaga : class, ISaga<TContext>
{
    /// <summary>
    /// The service collection to register saga steps and related services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// The parent registrar to allow chaining multiple saga configurations.
    /// </summary>
    public ISagasRegistrar Parent { get; }

    /// <summary>
    /// Allows chaining the configuration of another saga.
    /// </summary>
    /// <returns>The root saga registrar.</returns>
    ISagasRegistrar And();
}
