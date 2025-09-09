# SagaR

[![Build Status](https://github.com/lucafabbri/Sagar/actions/workflows/main.yml/badge.svg)](https://github.com/lucafabbri/Sagar/actions) [![GitHub release](https://img.shields.io/github/v/release/lucafabbri/Sagar)](https://github.com/lucafabbri/Sagar/releases) [![NuGet](https://img.shields.io/nuget/v/SagaR)](https://www.nuget.org/packages/SagaR)

**SagaR** is a lightweight, decoupled .NET library for orchestrating complex, multi-step business processes using the Saga pattern. It's designed to provide developers with a powerful yet simple tool for managing distributed transactions in modern architectures, like microservices, where traditional transaction mechanisms (e.g., 2PC) are not applicable.

## 🤔 Why SagaR?

Modern applications often need to perform complex operations that cross the boundaries of multiple services, databases, or third-party APIs. For example, creating an order might require reserving inventory, processing a payment, and scheduling a shipment. If any of these steps fail, how can you ensure the system doesn't remain in an inconsistent state (e.g., payment charged but goods not shipped)?

**SagaR** solves this by providing a dedicated orchestration layer that manages the entire process lifecycle. It allows you to:

* **Implement Sagas with Confidence**: Instead of writing complex state-machine logic and error handling within your services, you can define the entire workflow declaratively. This makes processes easier to understand, test, and maintain, reducing the likelihood of bugs.

* **Ensure Data Consistency**: The core feature of a saga is its ability to self-correct. SagaR automates the execution of compensating actions for every successfully completed operation. If something goes wrong, the library ensures the system returns to a valid state, achieving what is known as ""eventual consistency.""

* **Decouple Orchestration from Business Logic**: Following the Single Responsibility Principle, your command handlers remain focused exclusively on their specific business logic (e.g., processing a payment). They don't need to know what the previous or next step is. SagaR handles the flow, keeping your code cleaner, more modular, and reusable.

## ✨ Features

* **Saga Orchestration**: Provides a central coordinator (`ISagaOrchestrator`) that reliably executes the sequence of local transactions that make up the distributed process.

* **Automatic Compensation**: In case of a step failure, SagaR dynamically builds a stack of the completed operations and invokes their respective compensating actions in reverse order (LIFO - Last In, First Out), effectively undoing the changes made.

* **Fluent & Declarative API**: The configuration of sagas happens entirely at application startup (`Program.cs`) through an intuitive fluent API. This centralized approach improves code readability and maintainability.

* **Total Decoupling**: The library's core is the `ISagaStepRequestSender` abstraction. The orchestrator doesn't know how the steps are executed; it only dispatches requests through this interface. This allows you to use your preferred in-process messaging library without changing the orchestration.

* **Ready-to-use Integrations**: Provides native integration packages for the most common libraries in the .NET ecosystem, such as **MediatR** and **Concordia**, minimizing the boilerplate code needed for setup.

* **Transactional State Management**: Thanks to the optional but powerful integration with **TransactR**, your sagas can become persistent. The state can be saved after each step, allowing long-running processes to survive application restarts and be resumed later.

## ⚙️ How It Works

The logical flow of a saga managed by SagaR is simple and robust. The key components are:

* `ISaga`: Defines the ordered sequence of `ISagaStep`.
* `ISagaStep`: Represents a single operation, defining the action to be performed (`RequestFactory`) and the compensating action (`CompensateAsync`).
* `ISagaOrchestrator`: The engine that executes the saga.
* `ISagaStepRequestSender`: The abstraction that sends the requests for each step's execution.



The process unfolds as follows:

1.  A saga, defined as an ordered sequence of steps, is initiated.
2.  The `ISagaOrchestrator` starts executing the saga, processing one step at a time.
3.  For each step, it invokes the `RequestFactory` method to create a command/request object based on the saga's current context state.
4.  It sends this request using the configured `ISagaStepRequestSender` (e.g., MediatR's `Sender`). The corresponding handler executes the business logic.
5.  **Success Scenario**: If the step completes without exceptions, the orchestrator pushes it onto a ""completed steps"" stack and proceeds to the next one.
6.  **Failure Scenario**: If any step throws an exception, the orchestrator immediately stops the forward execution. It then starts popping steps from the stack (from last to first) and invokes the `CompensateAsync` method for each, ensuring that every change made is reverted.

## 🚀 Getting Started: Example with MediatR & TransactR

Here is a complete example of how to implement a ""Create Order"" saga with three steps: `CreateOrder`, `ProcessPayment`, and `DispatchOrder`.

### Installation

First, install the necessary NuGet packages. You will need the SagaR core and the integration package for your mediator.

```bash
# Core library
dotnet add package SagaR

# Integration for MediatR
dotnet add package SagaR.MediatR

# Optional: Integration for TransactR with MediatR for state persistence
dotnet add package SagaR.TransactR.MediatR
```

### 1. Define the Saga Context

The context is a POCO class that acts as a ""state container"" for the entire duration of the saga. Each step can read and write data to this object. By inheriting from `SagaTransactionContext`, we integrate it with TransactR, which will use it to save and restore the state.

```csharp
// CreateOrderContext.cs
using SagaR.TransactR;

public class CreateOrderContext : SagaTransactionContext<CreateOrderContext>
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }

    // The 'InitialStep' and 'Steps' properties are required by TransactR
    // to manage state progression persistently.
    public override string InitialStep => CreateOrderSteps.CreateOrder;
    public override string[] Steps => [CreateOrderSteps.CreateOrder, CreateOrderSteps.ProcessPayment, CreateOrderSteps.DispatchOrder];
}

// It's a good practice to define step names as constants
// to avoid typos and centralize their management.
public static class CreateOrderSteps
{
    public const string CreateOrder = ""1_create_order"";
    public const string ProcessPayment = ""2_process_payment"";
    public const string DispatchOrder = ""3_dispatch_order"";
}
```

### 2. Define MediatR Commands and Handlers

Each action is a standard MediatR command. `ISagaTransactiveRequest` is a ""marker"" interface that unifies the requirements of SagaR, MediatR, and TransactR.

```csharp
// ProcessPaymentRequest.cs
using MediatR;
using SagaR.TransactR.MediatR;
using TransactR;

public class ProcessPaymentRequest : ISagaTransactiveRequest<CreateOrderContext, Unit>
{
    public decimal Amount { get; set; }
    public Guid OrderId { get; set; }
    
    // These properties are populated by TransactR and SagaR
    public string TransactionId { get; set; }
    public IComparable Step { get; set; }
    public RollbackPolicy RollbackPolicy { get; set; }
    string ITransactionalRequest<string, CreateOrderContext>.Step => CreateOrderSteps.ProcessPayment;
}

// ProcessPaymentHandler.cs
public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentRequest, Unit>
{
    public Task<Unit> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
    {
        // We can simulate a failure to test the compensation logic
        if (ProcessPaymentStep.SimulateFailure)
        {
            throw new InvalidOperationException(""Credit card declined."");
        }

        Console.WriteLine($""EXECUTING: Payment of {request.Amount:C} processed."");
        // Business logic to charge the credit card would go here...
        return Task.FromResult(Unit.Value);
    }
}
```

*(Note: You would create a similar Request/Handler pair for each step of the saga).*

### 3. Define the Saga Steps

The `SagaStep` is the glue between the orchestration and the business logic. It has two main responsibilities:
1.  `RequestFactory`: To create the command to be sent for the step's execution.
2.  `CompensateAsync`: To define the undo logic for that step.

```csharp
// ProcessPaymentStep.cs
using SagaR.TransactR.MediatR;
using TransactR;

public class ProcessPaymentStep : MediatRTransactionalSagaStep<CreateOrderContext, ProcessPaymentRequest, Unit>
{
    public static bool SimulateFailure { get; set; } = false;

    public ProcessPaymentStep() : base(CreateOrderSteps.ProcessPayment) { }

    // This logic is executed only if a *subsequent* step fails.
    public override Task CompensateAsync(CreateOrderContext context, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($""COMPENSATING: Refunding payment of {context.Amount:C} for order {context.OrderId}."");
        Console.ResetColor();
        // Business logic to refund the payment would go here...
        return Task.CompletedTask;
    }

    // This factory creates the MediatR request using the saga's current state.
    public override ISagaStepRequest RequestFactory(CreateOrderContext context) => new ProcessPaymentRequest
    {
        Amount = context.Amount,
        OrderId = context.OrderId,
        TransactionId = context.TransactionId,
        Step = Step,
        RollbackPolicy = RollbackPolicy
    };
}
```

### 4. Define the Saga

The Saga class is the ""recipe"" for the process: it simply defines the ordered sequence of steps to be executed.

```csharp
// CreateOrderSaga.cs
using SagaR.TransactR;
using SagaR.TransactR.Abstractions;

public class CreateOrderSaga : TransactionalSaga<CreateOrderContext>
{
    public CreateOrderSaga(string transactionId)
        : base(transactionId, new ITransactionalSagaStep<CreateOrderContext>[] {
            new CreateOrderStep(),
            new ProcessPaymentStep(),
            new DispatchOrderStep()
        })
    {
    }
}
```

### 5. Configure Dependency Injection

In your `Program.cs`, use the fluent API to register the necessary services and configure the saga.

```csharp
// Program.cs
var services = new ServiceCollection();

services.AddLogging();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// Configure SagaR
services.UseSagas(sagas =>
{
    // 1. Specify that MediatR will be used to dispatch step commands.
    sagas.UseStepRequestSender<MediatRSagaStepRequestSender>()
         // 2. Map a context to its saga implementation.
         .MapSaga<CreateOrderContext, CreateOrderSaga>()
            // 3. (Optional) Enable the TransactR integration for this saga.
            .WithTransactR(transactr =>
            {
                // Configure where to persist the saga state.
                transactr.PersistedInMemory(); 
            });
});

var serviceProvider = services.BuildServiceProvider();
```

### 6. Execute the Saga

Finally, retrieve the `ISagaOrchestrator` from the DI container and run the saga, passing a saga instance and the initial context.

```csharp
// Program.cs
var orchestrator = serviceProvider.GetRequiredService<ISagaOrchestrator<CreateOrderContext>>();
var saga = new CreateOrderSaga(Guid.NewGuid().ToString());

var context = new CreateOrderContext
{
    OrderId = Guid.NewGuid(),
    ProductName = ""High-End Laptop"",
    Amount = 1500.50m
};
context.Initialize(Guid.NewGuid().ToString());

try
{
    await orchestrator.RunAsync(saga, context);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(""\nSaga completed successfully!"");
    Console.ResetColor();
}
catch (Exception)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($""\nSaga failed as expected. Compensation logic has been executed."");
    Console.ResetColor();
}
```

#### Example Output (Success Scenario)

```
EXECUTING: Order 1234-5678 created in database.
EXECUTING: Payment of $1,500.50 processed.
EXECUTING: Order 1234-5678 has been dispatched.

Saga completed successfully!
```

#### Example Output (Failure Scenario)

```
EXECUTING: Order 1234-5678 created in database.
EXECUTING: Payment failed. Throwing exception to trigger rollback...

Saga failed as expected. Compensation logic has been executed.
COMPENSATING: Refunding payment of $1,500.50 for order 1234-5678.
COMPENSATING: Deleting order 1234-5678 from database.
```

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](https://github.com/lucafabbri/Sagar/issues) to report a bug or suggest a new feature, or open a Pull Request directly.

## 💖 Show Your Support

Please give a ⭐️ if this project helped you! Your support is much appreciated.

## 📝 License

This project is licensed under the **MIT License**.