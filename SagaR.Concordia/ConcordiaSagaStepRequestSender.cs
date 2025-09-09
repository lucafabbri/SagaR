
using Concordia;

namespace SagaR.Concordia;

public class ConcordiaSagaStepRequestSender : ISagaStepRequestSender
{
    private readonly ISender _sender;

    public ConcordiaSagaStepRequestSender(ISender sender)
    {
        _sender = sender;
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : ISagaStepRequest
    {
        if(request is not IRequest typedRequest)
        {
            throw new InvalidOperationException($"The request of type {typeof(TRequest).FullName} does not implement IRequest.");
        }
        return _sender.Send(typedRequest, cancellationToken);
    }

    public Task<TResult> Send<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : ISagaStepRequest<TResult>
    {
        if(request is not IRequest<TResult> typedRequest)
        {
            throw new InvalidOperationException($"The request of type {typeof(TRequest).FullName} does not implement IRequest<{typeof(TResult).FullName}>.");
        }
        return _sender.Send(typedRequest, cancellationToken);
    }
}
