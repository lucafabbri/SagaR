using MediatR;

namespace Sagar.TransactR.Console.Test
{
    public class DispatchOrderHandler : IRequestHandler<DispatchOrderRequest, Unit>
    {
        public Task<Unit> Handle(DispatchOrderRequest request, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"EXECUTING: Order {request.OrderId} has been dispatched.");
            return Task.FromResult(Unit.Value);
        }
    }
}