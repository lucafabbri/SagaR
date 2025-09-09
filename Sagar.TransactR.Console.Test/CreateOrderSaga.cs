using SagaR.TransactR;
using SagaR.TransactR.Abstractions;

namespace Sagar.TransactR.Console.Test
{
    public class CreateOrderSaga : TransactionalSaga<CreateOrderContext>
    {
        public CreateOrderSaga() : this(Guid.NewGuid().ToString()) { }

        public CreateOrderSaga(string transactionId)
            : base(transactionId, [
                new CreateOrderStep(),
            new ProcessPaymentStep(),
            new DispatchOrderStep()
            ])
        {
        }

        public CreateOrderSaga(string transactionId, IEnumerable<ITransactionalSagaStep<CreateOrderContext>> steps) : base(transactionId, steps)
        {
        }

        public string TransactionId { get; private set; } = Guid.NewGuid().ToString();
    }
}