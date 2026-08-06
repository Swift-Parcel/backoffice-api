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

# SwiftParcel - QA Test Data Documentation

This document contains the pre-configured (seeded) data for testing the SwiftParcel Back-Office API (automated and manual integration tests). It focuses specifically on Parcel statuses and the Case lifecycle.

## 1. Users & Roles

| Username | Password | Role | Notes |
| :--- | :--- | :--- | :--- |
| readonly | ReadOnly123! | Read-Only | Read-only access for auditor/finance |
| operator | Operator123! | Operator | Standard case handler, sees own region |
| supervisor | Supervisor123! | Supervisor | Sees all regions, handles escalations |
| admin | admin | Admin | Full access to everything |
| handlerfull | HandlerFull123! | Operator | Handler who has reached the max (2) limit |

## 2. Customers

| Name | Email | VIP Status | Notes |
| :--- | :--- | :--- | :--- |
| Customer 1 | customer1@example.com | No | Default test customer |
| Customer 2 | customer2@example.com | Yes | For testing high-priority cases |

## 3. Parcels

| Tracking Number | Status (ParcelStatus) | Customer | Purpose |
| :--- | :--- | :--- | :--- |
| SP-20261016 | InTransit | Customer 1 | Normal delivery process testing |
| SP-20261017 | Delivered | Customer 1 | Successfully delivered parcel |
| SP-20261018 | Delivery attempt failed | Customer 2 | |
| SP-20261019 | Lost | Customer 2 | |

## 4. Cases

| Status | Type | Priority | Handler | Description / Auto-assignment goal |
| :--- | :--- | :--- | :--- | :--- |
| In Progress | Damaged | Medium | handlerfull | Limit test 1. (HandlerFull at max capacity) |
| In Progress | Delayed | High (VIP) | handlerfull | Limit test 2. (HandlerFull at max capacity) |
| Open | Lost | Medium | - (None) | For auto-assignment testing (upcoming feature) |
| Awaiting Customer | WrongAddress | Low | operator | Case waiting for customer reply |
| Resolved | Billing | Medium | operator | Solution within SLA deadline |
| Escalated | DeliveryChange | Critical | supervisor | Escalated case, visible/manageable only by supervisor |
| Closed | Other | Low | operator | Fully closed case |

## Useful Commands

- Stop all services:
  docker compose down

- Stop services and remove volumes (reset database data):
  docker compose down -v

- Inspect Primary Database via CLI:
  docker exec -it backoffice-api-swiftparcel-db-1 psql -U swiftparceldb -d SwiftParcelDb

- Inspect Legacy Database via CLI:
  docker exec -it backoffice-api-legacy-db-1 psql -U legacydb -d LegacySwiftParcelDb
