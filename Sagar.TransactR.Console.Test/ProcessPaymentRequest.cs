using MediatR;
using SagaR.TransactR.MediatR;
using TransactR;

namespace Sagar.TransactR.Console.Test
{
    public class ProcessPaymentRequest : ISagaTransactiveRequest<CreateOrderContext, Unit>
    {
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionId { get; set; }
        public IComparable Step { get; set; }
        public RollbackPolicy RollbackPolicy { get; set; }

        string ITransactionalRequest<string, CreateOrderContext>.Step => CreateOrderSteps.ProcessPayment;
    }
}