# Project Design and Implementation Details

The requirements for this project can be found in `.\DingTechTest\docs\DingTechTest.pdf`.

## Initial Approach and Pivot

I initially approached this project with the intention of building a Web API. However, during development, I noted a critical constraint in the instructions: *"You cannot change the public interface of this class."*

To honor this rule—specifically regarding `IAccountService`, which defines `PrintStatement` as returning `void`—I pivoted to creating a console application. In a real-world web application scenario, a `void` return type for a statement generation method would typically trigger an asynchronous process (like generating a PDF and sending an SMS/Email link via a messaging queue like RabbitMQ) rather than returning data directly in the HTTP response.

Since the requirements did not mandate a web application, I resorted to building a console application to strictly adhere to the provided interfaces while keeping the scope focused.

## Architecture and Future-Proofing

Despite pivoting to a console application, I maintained a robust, scalable structure:

- **Layered Architecture:** The console application (Presentation) strictly delegates to `Application.Domain` (Business Logic), which in turn relies on `Ding.Core` (Data Access). This structural boundary proves that the business logic can instantly be consumed by a REST API controller tomorrow with zero modifications.
- **Dependency Injection & SOLID Principles:** Every dependency (like `ILogger` or `IUnitOfWorkManager`) is injected via the constructor. The application firmly adheres to the Single Responsibility and Dependency Inversion principles by operating solely on abstractions instead of concrete implementations.
- **Repository & Unit of Work Design Pattern:** These patterns completely abstract Entity Framework Core away from the domain logic. They standardize data access without leaking database context (`DbContext`) concerns into the main service layer.
- **Factory Design Pattern:** Utilizing `IRepositoryFactory` allowed the application to dynamically spawn entirely fresh, disconnected generic Repositories per user operation. This factory abstraction was the critical backbone of our safe race condition isolation strategy.

By implementing these standards, the business logic and data access layers remain completely decoupled from the presentation layer (the console app). This ensures the application is future-proof and can be easily refactored into a Web API or another interface if needed down the line.

### Managing Race Conditions in a Fire-and-Forget Pattern

The required `IAccountService` interface enforces `void` return types for its methods:

```csharp
public interface IAccountService{
    public void Deposit(decimal amount);
    public void Withdraw(decimal amount);
    public void PrintStatement();
}
```

Implementing asynchronous logic within `void` methods results in a **fire-and-forget pattern**, meaning the caller cannot natively `await` their completion. This introduces a risk of race conditions—even in a single-user console app—if the user triggers consecutive operations before the previous ones have finished saving to the database.

To mitigate this without altering the interface, I used **Operation-level Service Instantiation**. By creating a fresh `AccountService` instance for every single user action, the application guarantees:

- **Isolated In-Memory State**: Each operation reads a fresh `AccountEntity` directly from the database during construction. This prevents stale data reads, cross-operational interference, and false "insufficient balance" errors.
- **Clean Separation of Concerns**: Each instance receives independent dependencies (e.g., a fresh `ILogger` and `UnitOfWorkManager`), ensuring consecutive operations don't bleed into or lock each other's Entity Framework contexts.
- **Simpler Mental Model**: By eliminating persistent shared state across user actions, the data flow is much easier to reason about and debug.

While this drastically reduces in-memory conflicts, it does not entirely solve database-level race conditions over shared records caused by the fire-and-forget nature of the methods. As a practical workaround within this console environment, requiring user input via `Console.ReadLine()` provides a natural blocking buffer, giving background database operations enough time to complete between menus.

*Note: If I had the privilege to change the provided interface, the objectively correct architectural solution would be to update these methods to return `Task` so they could be natively awaited.*

## Containerization

I chose to use Docker containers for the database (and the application itself) because:

- It guarantees the application will run consistently across different operating systems.
- It simplifies the deployment process across various environments, avoiding "it works on my machine" issues.
