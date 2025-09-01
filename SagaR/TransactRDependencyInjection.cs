using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace SagaR
{
    public static class TransactRDependencyInjection
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

        private class SagasRegistrar : ISagasRegistrar
        {
            private readonly IServiceCollection _services;
            public SagasRegistrar(IServiceCollection services) => _services = services;

            public ISagaMapConfigurator<TContext, TSaga> MapSaga<TContext, TSaga>()
                where TSaga : Saga<TContext>, ISaga<TContext>
            {
                return new SagaMapConfigurator<TContext, TSaga>(this, _services);
            }
        }

        private class SagaMapConfigurator<TContext, TSaga> : ISagaMapConfigurator<TContext, TSaga>
            where TSaga : Saga<TContext>, ISaga<TContext>
        {
            private readonly ISagasRegistrar _parent;
            private readonly IServiceCollection _services;
            private readonly List<Type> _stepTypes = new();

            public SagaMapConfigurator(ISagasRegistrar parent, IServiceCollection services)
            {
                _parent = parent;
                _services = services;

                _services.AddSingleton<ISaga<TContext>, TSaga>();
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
}

