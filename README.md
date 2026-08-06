# SwiftParcel Backoffice API

A backend service for managing parcel delivery operations, legacy system integrations, and administrative workflows.

---

## Tech Stack

- Framework: .NET 10 / C#
- Database: PostgreSQL 16 (Primary & Legacy DBs)
- Containerization: Docker & Docker Compose

---

## Getting Started

### Prerequisites
- Docker Desktop or Docker Engine with Docker Compose installed.
- .NET 10 SDK (optional, for local execution)

### Running the Application with Docker

1. Clone the repository:
   git clone <repository-url>
   cd backoffice-api

2. Start all services:
   docker compose up --build -d

3. Verify running containers:
   docker ps

---

## Services & Ports

| Service | Host Port | Internal Port | Description |
| :--- | :--- | :--- | :--- |
| Backoffice API | 3500 | 8080 | Main .NET REST API |
| SwiftParcel DB | 5433 | 5432 | Primary PostgreSQL Database |
| Legacy DB | 5434 | 5432 | Legacy PostgreSQL Database |
---

## API Documentation

When the application is running, Swagger UI is available at:
- http://localhost:3500/swagger
---

## Database Connections

### Connection Credentials

- SwiftParcel DB (Primary):
  - Host: localhost (External / IDE) | swiftparcel-db (Docker internal)
  - Port: 5433
  - Database: SwiftParcelDb
  - User: swiftparceldb
  - Password: password

- Legacy DB:
  - Host: localhost (External / IDE) | legacy-db (Docker internal)
  - Port: 5434
  - Database: LegacySwiftParcelDb
  - User: legacydb
  - Password: password

---

## Useful Commands

- Stop all services:
  docker compose down

- Stop services and remove volumes (reset database data):
  docker compose down -v

- Inspect Primary Database via CLI:
  docker exec -it backoffice-api-swiftparcel-db-1 psql -U swiftparceldb -d SwiftParcelDb

- Inspect Legacy Database via CLI:
  docker exec -it backoffice-api-legacy-db-1 psql -U legacydb -d LegacySwiftParcelDb
