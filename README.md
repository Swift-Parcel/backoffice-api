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

### API Authentication & Integrations

#### Java Portal Integration

- **Header:** `X-Api-Key`
- **Secret Value:** `SwiftParcel_Java_Integration_Shared_Secret_2026!`

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

| ID | Name | Email | VIP Status | Notes |
| :- | :--- | :--- | :--- | :--- |
| 1 | Janos Szabo | janos.szabo@gmail.com | No | |
| 2 | Petra Müller | petra.mueller@outlook.com | Yes | |
| 3 | Karel Svoboda | karel.svoboda@seznam.cz | No | |
| 4 | Ewa Kowalska | ewa.kowalska@wp.pl | No | |
| 5 | Hans Weber | hans.weber@gmx.at | Yes | |
| 7 | Lukas Bauer | l.bauer@gmail.com | No | |
| 8 | Piotr Zielinski | piotr.z@onet.pl | No | |
| 9 | Tamas Nagy | tamas.nagy@freemail.hu | No | |
| 10 | Maria Kiss | - | No | |
| 11 | Customer 1 | customer1@example.com | No | Default test customer |
| 12 | Customer 2 | customer2@example.com | Yes | |

## 3. Parcels

| ID | Tracking Number | Status | Customer | Purpose |
| :- | :--- | :--- | :--- | :--- |
| 1 | SP-20230101 | delivered | Customer 1 | |
| 2 | SP-20230102 | delivered | Customer 2 | |
| 3 | SP-20230103 | lost | Customer 3 | |
| 4 | SP-20230104 | damaged | Customer 4 | |
| 5 | SP-20230105 | in_transit | Customer 5 | |
| 6 | SP-20230106 | delivered | Customer 1 | |
| 7 | SP-20230107 | delivered | Customer 7 | |
| 8 | SP-20230108 | in_transit | Customer 2 | |
| 9 | SP-20230109 | delivered | Customer 4 | |
| 10 | SP-20261016 | in_transit | Customer 1 | Normal delivery process testing |
| 11 | SP-20261017 | delivered | Customer 1 | Successfully delivered parcel |
| 12 | SP-20261018 | delivery_attempt_failed | Customer 2 | |
| 13 | SP-20261019 | lost | Customer 2 | |

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

## 5. Tags

| ID | Name |
| :--- | :--- |
| 1 | investigation |
| 2 | refund |
| 3 | electronics |
| 4 | neighbor |
| 5 | duplicate_charge |
| 6 | sla_breach |
| 7 | fragile |
| 8 | insurance |
| 9 | tracking |
| 10 | stuck |
| 11 | compensation |
| 12 | international |
| 13 | long_running |
| 14 | wrong_item |
| 15 | swap |

## 6. Handlers

| ID | User ID | Department | Hire Date | Max Cases | Is Active |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | 1 | Customer Support | 2019-03-15 00:00:00+00 | 20 | true |
| 2 | 2 | Customer Support | 2020-06-15 00:00:00+00 | 25 | true |
| 3 | 3 | Escalations | 2021-01-10 00:00:00+00 | 15 | true |
| 4 | 4 | Customer Support | 2022-04-01 00:00:00+00 | 20 | true |
| 6 | 6 | Investigations | 2020-08-08 00:00:00+00 | 10 | true |
| 7 | 16 | Customer Support | 2024-08-07 12:54:39.090028+00 | 2 | true |
| 8 | 14 | Customer Support | 2025-08-07 12:54:39.200401+00 | 10 | true |
| 9 | 15 | Escalations | 2024-08-07 12:54:39.200403+00 | 5 | true |

## 7. Regions

| ID | Name | Country Code | Business Hours | Manager Email | Is Active | Business Days |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | Budapest | HU | 08:00:00 - 18:00:00 | bp.manager@swiftparcel.com | true | {monday,tuesday,wednesday,thursday,friday} |
| 2 | Wien | AT | 08:00:00 - 17:00:00 | vienna.mgr@swiftparcel.com | true | {monday,tuesday,wednesday,thursday,friday} |
| 3 | Prague | CZ | 09:00:00 - 17:30:00 | prague.manager@swiftparcel.com | true | {monday,tuesday,wednesday,thursday,friday} |
| 4 | Warsaw | PL | 08:00:00 - 17:00:00 | warsaw.mgr@swiftparcel.com | true | {monday,tuesday,wednesday,thursday,friday} |
| 5 | Graz | AT | 08:00:00 - 17:00:00 | graz.mgr@swiftparcel.com | true | {monday,tuesday,wednesday,thursday,friday} |
| 6 | Linz | AT | 08:00:00 - 17:00:00 | | true | {monday,tuesday,wednesday,thursday,friday} |
| 7 | Bratislava | SK | 08:00:00 - 16:30:00 | bratislava.mgr@swiftparcel.com | false | {monday,tuesday,wednesday,thursday,friday} |


## Case assignment

| IsEscalated | CaseType | Department |
| :--- | :--- | :--- |
| `true` | Any | Escalations |
| `false` | Lost | Investigations |
| `false` | Any other than lost | Customer Support |

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
