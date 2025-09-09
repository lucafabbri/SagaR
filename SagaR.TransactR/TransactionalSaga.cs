using SagaR.TransactR.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactR;

namespace SagaR.TransactR;

public abstract class TransactionalSaga<TContext> : Saga<TContext>, ITransactionalSaga<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    public string TransactionId { get; private set; } = string.Empty;

    public TransactionalSaga(string transactionId, IEnumerable<ITransactionalSagaStep<TContext>> steps) : base(steps.Select(_ => { _.WithTransactionId(transactionId); return _; }))
    {
        TransactionId = transactionId;
    }
}
