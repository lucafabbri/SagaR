using MediatR;
using SagaR.TransactR.MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    public class CreateOrderRequest : ISagaTransactiveRequest<CreateOrderContext, Unit>
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public string TransactionId { get; set; }
        public IComparable Step { get; set; }
        public RollbackPolicy RollbackPolicy { get; set; }
        string ITransactionalRequest<string, CreateOrderContext>.Step => CreateOrderSteps.CreateOrder;
    }
}