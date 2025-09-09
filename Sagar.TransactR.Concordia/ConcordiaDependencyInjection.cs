using Microsoft.Extensions.DependencyInjection;
using SagaR.TransactR.Abstractions;
using SagaR.TransactR.Internal;
using TransactR;
using TransactR.Concordia;

namespace SagaR.TransactR.Concordia;

public static class ConcordiaDependencyInjection
{
    public static ISagaMapConfigurator<TContext, TSaga> WithTransactR<TContext, TSaga>(
        this ISagaMapConfigurator<TContext, TSaga> configurator,
        Action<ITransactorBuilder<string, TContext>> configureTransactr)
        where TContext : class, ITransactionContext<string, TContext>, new()
        where TSaga : class, ITransactionalSaga<TContext>, new()
    {
        // Override the default SagaR orchestrator with our transactional version.
        configurator.Services.AddScoped<ISagaOrchestrator<TContext>, TransactionalSagaOrchestrator<TContext>>();

        // Start the TransactR configuration chain.
        var transactrBuilder = configurator.Services
            .AddTransactR()
            .OnConcordia()
            .HasState<string, TContext>()
            .RestoredBy<EmptyStateRestorer<string, TContext>>();

        var tempSaga = new TSaga();

        foreach (var step in tempSaga.Steps)
        {
            var requestType = typeof(ISagaTransactiveRequest<,>);
            //check step.RequestType implements ISagaTransactiveRequest<TContext, TState, TResponse>
            if (step.RequestType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestType))
            {
                transactrBuilder = transactrBuilder.Surround(step.RequestType);
            }
        }

        configureTransactr?.Invoke(transactrBuilder);

        return configurator;
    }
}

