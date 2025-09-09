using MediatR;

namespace SagaR.MediatR;


public class MediatRSagaStepRequestSender : ISagaStepRequestSender
{
    private readonly ISender _sender;

    public MediatRSagaStepRequestSender(ISender sender)
    {
        _sender = sender;
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ISagaStepRequest
    {
        return _sender.Send(request, cancellationToken);
    }
}
