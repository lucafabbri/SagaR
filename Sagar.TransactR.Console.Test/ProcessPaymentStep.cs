using MediatR;
using SagaR;
using SagaR.TransactR.MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    // STEP 2: Processare il pagamento
    public class ProcessPaymentStep : MediatRTransactionalSagaStep<CreateOrderContext, ProcessPaymentRequest, Unit>
    {
        public decimal Amount { get; private set; }
        public Guid OrderId { get; private set; }
        public static bool SimulateFailure { get; set; } = false; // Flag per testare il rollback

        public ProcessPaymentStep() : base(CreateOrderSteps.ProcessPayment, RollbackPolicy.RollbackToCurrentStep) { }

        public override Task CompensateAsync(CreateOrderContext context, CancellationToken cancellationToken = default)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"COMPENSATING: Refunding payment of {context.Amount:C} for order {context.OrderId}.");
            System.Console.ResetColor();
            return Task.CompletedTask;
        }

        public override ISagaStepRequest RequestFactory(CreateOrderContext context) => new ProcessPaymentRequest
        {
            Amount = context.Amount,
            OrderId = context.OrderId,
            TransactionId = context.TransactionId,
            Step = Step,
            RollbackPolicy = RollbackPolicy
        };
    }
}