using MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, Unit>
    {
        private readonly ITransactionContextProvider<string, CreateOrderContext> _contextProvider;

        public CreateOrderHandler(ITransactionContextProvider<string, CreateOrderContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Task<Unit> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            _contextProvider.Context.OrderId = request.OrderId; // Salva l'ID dell'ordine nello stato persistente

            System.Console.WriteLine($"EXECUTING: Order {request.OrderId} for product '{request.ProductName}' created in database.");
            return Task.FromResult(Unit.Value);
        }
    }
}