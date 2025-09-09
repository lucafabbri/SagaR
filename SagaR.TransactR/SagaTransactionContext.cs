using TransactR;

namespace SagaR.TransactR;

/// <summary>
/// An internal implementation of ITransactionContext tailored for sagas.
/// It determines the transaction outcome based on the saga's step progression.
/// </summary>
public abstract class SagaTransactionContext<TContext> : StringTransactionContext<TContext>
    where TContext : class, ITransactionContext<string, TContext>, new()
{
    /// <summary>
    /// Evaluates the outcome of a saga step. If the current step is the last one in the defined sequence,
    /// the saga is considered Completed; otherwise, it is InProgress.
    /// </summary>
    public override TransactionOutcome EvaluateResponse(object? response = null)
    {
        var currentStep = Step;
        var lastStep = Steps?.LastOrDefault();

        if (string.IsNullOrEmpty(currentStep) || string.IsNullOrEmpty(lastStep))
        {
            // If steps are not defined or state is invalid, complete to avoid infinite loops.
            return TransactionOutcome.Completed;
        }

        return currentStep == lastStep ? TransactionOutcome.Completed : TransactionOutcome.InProgress;
    }
}

