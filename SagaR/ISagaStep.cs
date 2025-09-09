namespace SagaR;

public interface ISagaStep<in TContext>
{
    string Identifier { get; }

    Type RequestType { get; }

    ISagaStepRequest RequestFactory(TContext context);

    /// <summary>
    /// The compensation logic to execute if a subsequent step fails.
    /// </summary>
    /// <param name="context">The current transaction context.</param>
    Task CompensateAsync(TContext context, CancellationToken cancellationToken = default);
}

public interface ISagaStep<in TContext, TRequest> : ISagaStep<TContext>
    where TRequest : ISagaStepRequest
{
    new TRequest RequestFactory(TContext context);
}

public interface ISagaStep<in TContext, TRequest, TResponse> : ISagaStep<TContext, TRequest>
    where TRequest : ISagaStepRequest<TResponse>
{
    new TRequest RequestFactory(TContext context);
}
