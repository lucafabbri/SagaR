namespace SagaR;

public interface ISagaStepRequestSender
{
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ISagaStepRequest;
}
