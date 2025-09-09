using MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentRequest, Unit>
    {
        private readonly ITransactionContextProvider<string, CreateOrderContext> _contextProvider;

        public ProcessPaymentHandler(ITransactionContextProvider<string, CreateOrderContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Task<Unit> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            if (ProcessPaymentStep.SimulateFailure)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine("EXECUTING: Payment failed. Throwing exception to trigger rollback...");
                System.Console.ResetColor();
                throw new InvalidOperationException("Credit card declined.");
            }

            _contextProvider.Context.IsPaid = true;

            System.Console.WriteLine($"EXECUTING: Payment of {request.Amount:C} for order {_contextProvider.Context.OrderId} processed successfully.");
            return Task.FromResult(Unit.Value);
        }
    }
}