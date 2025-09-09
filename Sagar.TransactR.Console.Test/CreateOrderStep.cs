using MediatR;
using SagaR;
using SagaR.TransactR.MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    // 4. Step e Handler
    // STEP 1: Creare l'ordine
    public class CreateOrderStep : MediatRTransactionalSagaStep<CreateOrderContext, CreateOrderRequest, Unit>
    {
        public CreateOrderStep() : base(CreateOrderSteps.CreateOrder, RollbackPolicy.RollbackToCurrentStep)
        {
        }


        public override Task CompensateAsync(CreateOrderContext context, CancellationToken cancellationToken = default)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"COMPENSATING: Deleting order {context.OrderId} from database.");
            System.Console.ResetColor();
            return Task.CompletedTask;
        }

        public override ISagaStepRequest RequestFactory(CreateOrderContext context) => new CreateOrderRequest
        {
            OrderId = context.OrderId,
            ProductName = context.ProductName,
            TransactionId = context.TransactionId,
            Step = Step,
            RollbackPolicy = RollbackPolicy
        };
    }
}