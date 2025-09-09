namespace SagaR;

public interface ISaga<TContext> { 
    IEnumerable<ISagaStep<TContext>> Steps { get; }
}
