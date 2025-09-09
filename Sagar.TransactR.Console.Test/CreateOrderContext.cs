using SagaR.TransactR;

namespace Sagar.TransactR.Console.Test
{
    public class CreateOrderContext : SagaTransactionContext<CreateOrderContext>
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }

        public override string InitialStep => CreateOrderSteps.CreateOrder;

        public override string[] Steps => [CreateOrderSteps.CreateOrder, CreateOrderSteps.ProcessPayment, CreateOrderSteps.DispatchOrder];
    }
}