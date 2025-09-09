using Microsoft.Extensions.DependencyInjection;
using SagaR;
using SagaR.MediatR;
using SagaR.TransactR.MediatR;

namespace Sagar.TransactR.Console.Test
{

    // --- FINE: Scenario di Esempio ---

    public class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("--- ESECUZIONE SAGA (SUCCESS SCENARIO) ---");
            await ExecuteSaga(simulateFailure: false);

            System.Console.WriteLine("\n---------------------------------------------\n");

            System.Console.WriteLine("--- ESECUZIONE SAGA (FAILURE SCENARIO) ---");
            await ExecuteSaga(simulateFailure: true);
        }

        private static async Task ExecuteSaga(bool simulateFailure)
        {
            ProcessPaymentStep.SimulateFailure = simulateFailure;

            var services = new ServiceCollection();

            // 1. Configurazione dipendenze standard
            services.AddLogging();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

            // 2. Configurazione SagaR e TransactR con la nuova API fluente
            services.UseSagas(sagas =>
            {
                sagas.UseStepRequestSender<MediatRSagaStepRequestSender>()
                     .MapSaga<CreateOrderContext, CreateOrderSaga>()
                        .WithTransactR(configureTransactr =>
                        {
                            configureTransactr.PersistedInMemory();
                        });
            });


            var serviceProvider = services.BuildServiceProvider();

            // 3. Esecuzione Saga
            var orchestrator = serviceProvider.GetRequiredService<ISagaOrchestrator<CreateOrderContext>>();
            var saga = new CreateOrderSaga(Guid.NewGuid().ToString());

            var context = new CreateOrderContext
            {
                OrderId = Guid.NewGuid(),
                ProductName = "Laptop Super Performante",
                Amount = 1500.50m
            };
            context.Initialize(Guid.NewGuid().ToString()); // Inizializza con un TransactionId

            try
            {
                await orchestrator.RunAsync(saga, context);
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("\nSaga completata con successo!");
                System.Console.ResetColor();
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.WriteLine($"\nSaga fallita come previsto: {ex.Message}");
                System.Console.ResetColor();
            }
        }
    }
}