using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SagaR
{
    /// <summary>
    /// The default implementation of the saga orchestrator. It executes steps sequentially
    /// and runs compensation logic in reverse order if a step fails.
    /// </summary>
    public class SagaOrchestrator<TContext> : ISagaOrchestrator<TContext>
    {
        private readonly ILogger<SagaOrchestrator<TContext>> _logger;

        public SagaOrchestrator(ILogger<SagaOrchestrator<TContext>> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(ISaga<TContext> saga, TContext context)
        {
            var completedSteps = new Stack<ISagaStep<TContext>>();

            foreach (var step in saga.Steps)
            {
                try
                {
                    _logger.LogInformation("Executing saga step {StepType}...", step.GetType().Name);
                    await step.ExecuteAsync(context);
                    completedSteps.Push(step);
                    _logger.LogInformation("Saga step {StepType} completed successfully.", step.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in saga step {StepType}. Initiating compensation.", step.GetType().Name);
                    await CompensateAsync(completedSteps, context);
                    // Re-throw the original exception to notify the caller (e.g., the TransactionalBehavior)
                    throw;
                }
            }
        }

        private async Task CompensateAsync(Stack<ISagaStep<TContext>> completedSteps, TContext context)
        {
            _logger.LogInformation("Starting compensation for {CompletedStepsCount} completed steps.", completedSteps.Count);

            while (completedSteps.Count > 0)
            {
                var stepToCompensate = completedSteps.Pop();
                try
                {
                    _logger.LogInformation("Compensating for step {StepType}...", stepToCompensate.GetType().Name);
                    await stepToCompensate.CompensateAsync(context);
                    _logger.LogInformation("Compensation for step {StepType} completed.", stepToCompensate.GetType().Name);
                }
                catch (Exception compEx)
                {
                    // If a compensation fails, it's a critical, unrecoverable error.
                    _logger.LogCritical(compEx, "A critical error occurred during compensation for step {StepType}. Manual intervention may be required.", stepToCompensate.GetType().Name);
                    // We re-throw because the system is in an inconsistent state.
                    throw;
                }
            }
        }
    }
}

