# Ding Bank Management System

A high-performance **Bank Account Management System** built with **.NET 10.0** and **PostgreSQL**. The project implements a robust banking domain service using clean architecture, the repository pattern, and a custom Unit of Work management system.

---

## ✨ Features

- **Account Operations**: Support for deposits and withdrawals with real-time balance tracking.
- **Detailed Statements**: Generation of transaction histories including transaction-time balances.
- **Persistence Layer**: Custom Repository pattern implementation that abstracts database complexity.
- **Robust Entity Management**: Integrated tracking and detachment mechanism to ensure safe concurrent operations in shared context scenarios.
- **Auto-Seeding**: Automatic database schema creation and data seeding for development and testing.

---

## 🛠️ Tech Stack

### Backend

- **Framework**: [.NET 10.0](https://dotnet.microsoft.com/) Console/Host-based application.
- **Persistence**: [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) using the **Npgsql** provider.
- **Architecture**: Domain-Driven Design (DDD) principles with a focus on core domain abstractions.
- **Dependency Injection**: Integrated Microsoft Dependency Injection for service lifecycle management.

### Infrastructure

- **Primary Database**: [PostgreSQL 17+](https://www.postgresql.org/)
- **Containerization**: [Docker](https://www.docker.com/) & [Docker Compose](https://docs.docker.com/compose/) for consistent environments.

---

## 🏗️ Architecture & Dependencies

The project follows a modular clean architecture:

![Architecture Diagram](./docs/Architecture%26Dependencies.svg)

### Key Libraries

- **Ding.Core**: Contains generic repository base classes and the Unit of Work manager.
- **Application.Domain**: Houses the `IAccountService` and `ICustomerService` implementations.
- **Application.Entities**: Defines the `Account`, `Customer`, and `Transaction` database schema.

---

## 🚀 Getting Started

To run the project locally, follow these steps:

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run with Docker Compose

1. **Navigate to the container folder**:

   ```powershell
   cd docker-containers
   ```

2. **Start the environment**:

   ```powershell
   docker-compose up -d --build
   ```

3. **Connect to the Console**:
   The application will run inside the `ding-tech-console` container.

---

## 📁 Project Structure

- `src/Application.Console`: CLI entry point and application initialization.
- `src/Application.Domain`: Business logic interfaces and service implementations.
- `src/Application.Entities`: Entity definitions and the `ApplicationDbContext`.
- `src/Ding.Core`: Shared persistence logic, repository base, and transaction management.
- `docker-containers/`: Configuration for PostgreSQL and application containerization.

---

## 📝 License

This project is licensed under the [MIT License](LICENSE).

---

> 💡 **Curious about the architectural decisions and race condition handling?**  
> [Dive into the Design Documentation (`docs/README.md`)](docs/README.md) to explore the thought process, pattern selections, and how the firm interface constraints were successfully navigated!
