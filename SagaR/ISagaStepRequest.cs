namespace SagaR;

public interface ISagaStepRequest { }

public interface ISagaStepRequest<TResponse> : ISagaStepRequest { }
