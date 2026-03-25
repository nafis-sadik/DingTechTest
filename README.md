# DingTechTest - Multi-Tenant Billing System

A modern, high-performance **Multi-Tenant Billing System** built with **.NET 10.0** and **PostgreSQL**. This project follows clean architecture principles and is containerized for easy development and deployment.

---

## 🛠️ Tech Stack

### Backend

- **Framework**: [.NET 10.0 (Preview/Latest)](https://dotnet.microsoft.com/)
- **API**: ASP.NET Core Web API
- **ORM**: [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) with **Npgsql**
- **Architecture**: Domain-Driven Design (DDD)
- **API Documentation**: [Swagger / OpenAPI](https://swagger.io/specification/)
- **Dependency Injection**: Integrated ASP.NET Core DI
- **Containerization**: [Docker](https://www.docker.com/) & [Docker Compose](https://docs.docker.com/compose/)

### Frontend (Upcoming)

- **Framework**: [Angular](https://angular.io/) (Planned)

### Database

- **Primary DB**: [PostgreSQL 17+](https://www.postgresql.org/)
- **Identity & Auth**: Integrated Identity System (Consolidated into Application Database)

---

## 🚀 Getting Started

To get the project up and running locally, follow these steps:

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (if running without Docker)
- [PostgreSQL](https://www.postgresql.org/download/) (if running without Docker)

### Run with Docker Compose (Recommended)

This is the easiest way to start both the backend API and the database.

1. **Navigate to the Docker directory**:

   ```powershell
   cd docker-containers
   ```

2. **Configure Environment Variables**:
   - Open the `.env` file in the `docker-containers` folder.
   - Adjust the credentials or ports if necessary. (Note: `.env` is ignored by git for security).

3. **Start the Services**:

   ```powershell
   docker-compose up -d --build
   ```

   - **Backend API**: Available at [http://localhost:8080](http://localhost:8080)
   - **Postgres DB**: Available at `localhost:5432`

---

## 🏗️ Architecture & Dependencies

The project follows a modular architecture where each layer has a specific responsibility:

```mermaid
graph TD
    API["Application.API (Web API)"] 
    API --> Domain["Application.Domain (Business Logic)"]
    Domain --> Entities["Application.Entities (Data Models)"]
    Entities --> Core["System.Core (Base Components)"]
    
    subgraph "External Dependencies"
        Core --> Postgres["Postgres / Npgsql"]
        Core --> EF["EF Core 10.0"]
    end

    classDef primary fill:#e1f5fe,stroke:#01579b,stroke-width:2px;
    classDef secondary fill:#f3e5f5,stroke:#4a148c,stroke-width:2px;
    class API,Domain,Entities,Core primary;
    class Postgres,EF secondary;
```

---

## 📁 Project Structure

- `src/Application.API`: The main entry point for the RESTful API service.
- `src/Application.Domain`: Contains business logic, interfaces, and domain models.
- `src/Application.Entities`: Database models and Entity Framework DbContext.
- `src/System.Core`: Shared libraries, generic repositories, and utility functions.
- `docker-containers/`: Docker Compose and environment configurations.

---

## 📝 License

This project is licensed under the [MIT License](LICENSE).