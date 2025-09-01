# SagaR

[![CI/CD Status](https://github.com/lucafabbri/SagaR/actions/workflows/main.yml/badge.svg)](https://github.com/lucafabbri/SagaR/actions/workflows/main.yml)
[![GitHub release](https://img.shields.io/github/v/release/lucafabbri/SagaR)](https://github.com/lucafabbri/SagaR/releases/latest)

**NuGet Package:**

[![NuGet](https://img.shields.io/nuget/v/SagaR.svg)](https://www.nuget.org/packages/SagaR/)

**SagaR** is a lightweight, decoupled .NET library for orchestrating complex processes based on the Saga pattern. It provides a fluent API and native integration with Dependency Injection to allow you to declaratively define the sequence of operations and their corresponding compensation logic.

SagaR is designed to be agnostic of the state management framework, but it can be used in perfect synergy with libraries like **TransactR** to add a robust rollback mechanism to your sagas.

## Key Concepts

* **Saga Pattern**: Manages distributed transactions by executing a sequence of local transactions. If a transaction fails, the saga executes a series of compensating transactions to undo the preceding changes.

* **Declarative Definition**: Sagas are defined cleanly and centrally during the Dependency Injection setup, keeping the business logic in handlers lean and focused.

* **Dependency Injection-first**: Saga steps are resolved from the DI container, which means they can have their own dependencies (services, repositories, etc.) automatically injected.

* **Orchestration**: The `ISagaOrchestrator` service is responsible for executing the steps in sequence and invoking compensation actions in reverse order in case of failure.

## Installation

Install the NuGet package into your application.

```
dotnet add package SagaR
```

## Usage Example

### 1. Define the Steps

Each step is a class that implements `ISagaStep<TContext>` and contains the execution and compensation logic.

```
public class ReserveInventoryStep : ISagaStep<OrderContext>
{
    private readonly IInventoryService _inventory;
    public ReserveInventoryStep(IInventoryService inventory) => _inventory = inventory;
    
    public Task ExecuteAsync(OrderContext context) => _inventory.ReserveItemAsync(context.State.ItemId);
    public Task CompensateAsync(OrderContext context) => _inventory.ReleaseItemAsync(context.State.ItemId);
}

public class ProcessPaymentStep : ISagaStep<OrderContext>
{
    private readonly IPaymentService _payment;
    public ProcessPaymentStep(IPaymentService payment) => _payment = payment;

    public Task ExecuteAsync(OrderContext context) => _payment.ProcessPaymentAsync(context.State.Amount);
    public Task CompensateAsync(OrderContext context) => _payment.RefundPaymentAsync(context.State.Amount);
}
```

### 2. Define the Saga

Create a class that inherits from `Saga<TContext>` and receives the steps via its constructor.

```
public class OrderCreationSaga : Saga<OrderContext>
{
    // The DI container will inject all registered ISagaStep<OrderContext> for this saga.
    public OrderCreationSaga(IEnumerable<ISagaStep<OrderContext>> steps) : base(steps) { }
}
```

### 3. Configuration (Dependency Injection)

Use the fluent `UseSagas` API to register and define your saga.

```
// Program.cs
services.UseSagas(sagas =>
{
    sagas.MapSaga<OrderContext, OrderCreationSaga>()
         .WithStep<ReserveInventoryStep>()  // Registers the step and associates it with the saga
         .WithStep<ProcessPaymentStep>();
});
```

### 4. Saga Execution

Inject `ISaga<TContext>` and `ISagaOrchestrator<TContext>` into your handler or service to execute the saga.

```
public class CreateOrderHandler
{
    private readonly ISaga<OrderContext> _saga;
    private readonly ISagaOrchestrator<OrderContext> _orchestrator;
    private readonly MyContextProvider _contextProvider; // e.g., ITransactionContextProvider from TransactR

    public CreateOrderHandler(
        ISaga<OrderContext> saga, 
        ISagaOrchestrator<OrderContext> orchestrator,
        MyContextProvider contextProvider)
    {
        _saga = saga;
        _orchestrator = orchestrator;
        _contextProvider = contextProvider;
    }

    public async Task Handle(CreateOrderCommand command)
    {
        var context = _contextProvider.Context;
        // Initialize the context with data from the command...

        await _orchestrator.RunAsync(_saga, context);
    }
}
```

## Integration with TransactR

SagaR and TransactR are designed to work together. You can use a `TransactionContext` from TransactR as the `TContext` for your saga. This way, if any saga step throws an exception, the `TransactionalBehavior` from TransactR will catch it and apply the rollback policy to the entire transaction state.
```
