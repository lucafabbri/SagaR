using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace SagaR;

public static class SagaRDependencyInjection
{
    /// <summary>
    /// Registers TransactR saga services and allows for fluent configuration of sagas.
    /// </summary>
    /// <param name="services">The IServiceCollection.</param>
    /// <param name="configure">An action to configure all sagas for the application.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    public static IServiceCollection UseSagas(this IServiceCollection services, Action<ISagasRegistrar> configure)
    {
        // Register the default orchestrator if not already registered.
        services.TryAddScoped(typeof(ISagaOrchestrator<>), typeof(SagaOrchestrator<>));

        var registrar = new SagasRegistrar(services);
        configure(registrar);

        return services;
    }

    internal class SagasRegistrar : ISagasRegistrar
    {
        private readonly IServiceCollection _services;

        public IServiceCollection Services => _services;

        public SagasRegistrar(IServiceCollection services) => _services = services;

        public ISagaMapConfigurator<TContext, TSaga> MapSaga<TContext, TSaga>()
            where TContext : class, new()
            where TSaga : class, ISaga<TContext>
        {
            return new SagaMapConfigurator<TContext, TSaga>(this, _services);
        }

        public ISagasRegistrar UseStepRequestSender<TRequestSender>()
            where TRequestSender : class, ISagaStepRequestSender
        {
            _services.AddTransient<ISagaStepRequestSender, TRequestSender>();
            return this;
        }
    }

    internal class SagaMapConfigurator<TContext, TSaga> : ISagaMapConfigurator<TContext, TSaga>
        where TContext : class, new()
        where TSaga : class, ISaga<TContext>
    {
        internal readonly ISagasRegistrar _parent;
        internal readonly IServiceCollection _services;
        internal readonly List<Type> _stepTypes = new();

        public IServiceCollection Services => _services;

        public ISagasRegistrar Parent => _parent;

        internal SagaMapConfigurator(ISagaMapConfigurator<TContext, TSaga> sagaMapConfigurator)
        {
            _parent = sagaMapConfigurator.Parent;
            _services = sagaMapConfigurator.Services;
        }

        public SagaMapConfigurator(ISagasRegistrar parent, IServiceCollection services)
        {
            _parent = parent;
            _services = services;

            _services.AddScoped<ISaga<TContext>, TSaga>();
        }

        public ISagaMapConfigurator<TContext, TSaga> WithStep<TStep>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TStep : class, ISagaStep<TContext>
        {
            // Automatically register the step type with the specified lifetime.
            _services.Add(new ServiceDescriptor(typeof(ISagaStep<TContext>), typeof(TStep), lifetime));

            _stepTypes.Add(typeof(TStep));
            return this;
        }

        public ISagasRegistrar And() => _parent;
    }
}

