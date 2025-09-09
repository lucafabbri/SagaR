using MediatR;
using SagaR;
using SagaR.TransactR.MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    // STEP 3: Spedire l'ordine
    public class DispatchOrderStep : MediatRTransactionalSagaStep<CreateOrderContext, DispatchOrderRequest, Unit>
    {
        public Guid OrderId { get; private set; }

        public DispatchOrderStep() : base(CreateOrderSteps.DispatchOrder, RollbackPolicy.RollbackToCurrentStep) { }

        public override Task CompensateAsync(CreateOrderContext context, CancellationToken cancellationToken = default)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"COMPENSATING: Cancelling dispatch for order {context.OrderId}.");
            System.Console.ResetColor();
            return Task.CompletedTask;
        }

        public override ISagaStepRequest RequestFactory(CreateOrderContext context) => new DispatchOrderRequest
        {
            OrderId = context.OrderId,
            TransactionId = context.TransactionId,
            Step = Step,
            RollbackPolicy = RollbackPolicy
        };
    }
}