# Project Design and Implementation Details

The requirements for this project can be found in `.\DingTechTest\docs\DingTechTest.pdf`.

## Initial Approach and Pivot

I initially approached this project with the intention of building a Web API. However, during development, I noted a critical constraint in the instructions: *"You cannot change the public interface of this class."*

To honor this rule—specifically regarding `IAccountService`, which defines `PrintStatement` as returning `void`—I pivoted to creating a console application. In a real-world web application scenario, a `void` return type for a statement generation method would typically trigger an asynchronous process (like generating a PDF and sending an SMS/Email link via a messaging queue like RabbitMQ) rather than returning data directly in the HTTP response.

Since the requirements did not mandate a web application, I resorted to building a console application to strictly adhere to the provided interfaces while keeping the scope focused.

## Architecture and Future-Proofing

Despite pivoting to a console application, I maintained a robust, scalable structure:

- **Layered Architecture**
- **SOLID Principles**
- **Repository Design Pattern**
- **Factory Design Pattern**

By implementing these standards, the business logic and data access layers remain completely decoupled from the presentation layer (the console app). This ensures the application is future-proof and can be easily refactored into a Web API or another interface if needed down the line.

## Containerization

I chose to use Docker containers for the database (and the application itself) because:

- It guarantees the application will run consistently across different operating systems.
- It simplifies the deployment process across various environments, avoiding "it works on my machine" issues.
