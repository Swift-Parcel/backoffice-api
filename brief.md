# Design Decisions and Trade-offs

## CQRS with MediatR

The back-office application is built on a Clean Architecture foundation utilizing the Command Query Responsibility Segregation (CQRS) pattern via MediatR.

Read and write operations are strictly separated into Queries and Commands.
This introduces additional boilerplate, requiring separate records, handlers, and validators for every action, but significantly improves testability and enforces a clear separation of concerns. It also enables the use of global pipeline behaviors for cross-cutting concerns like authorization and validation.



## Database Design & EF Core Integration

The data layer utilizes Entity Framework Core mapped to PostgreSQL, leveraging Postgres-specific features for enhanced data integrity.

The schema enforces 3NF. We utilize native Postgres enums for structured statuses and the `citext` extension for case-insensitive email constraints. System configuration is stored natively as `jsonb`.
Utilizing provider-specific features ties the application closer to PostgreSQL, making database portability slightly more difficult. However, the performance gains and strict schema enforcement at the database level far outweigh this limitation for a mid-size logistics system.

## Concurrency Control in Case Assignment

Assigning cases to handlers requires strict enforcement of workload limits (`MaxCases`).

Implemented pessimistic concurrency control using a `FOR UPDATE` row-level lock within `HandlerRepository.GetWithLockAndCasesAsync`.
Locking rows during assignment can cause slight database contention under massive simultaneous assignment loads. However, it completely eliminates race conditions where multiple concurrent requests might exceed a handler's maximum capacity.



## Dual Security & Authentication Model

The system serves two completely different consumers: internal staff and the external Customer Portal.

Implemented a dual authentication strategy. Internal endpoints utilize standard JWT Bearer tokens carrying granular claims. Integration endpoints are secured via a lightweight, shared API Key using a custom `[ApiKeyAuth]` filter.
Managing two authentication mechanisms increases the security surface area slightly. Yet, forcing the Java application to continually negotiate JWTs for machine-to-machine communication would add unnecessary complexity and overhead to the integration contract.

## Webhook Notifications & Integration Resiliency

To fulfill the requirement of notifying the Java application about parcel and case updates, an outgoing HTTP client is used.

Implemented a fire-and-forget `WebhookClient` that dispatches JSON payloads to the Customer Portal when domain events occur. If the request fails, the error is logged and execution continues seamlessly.
Without a dedicated message broker, guaranteed delivery is not currently enforced. If the Java service is down during a status change, the webhook is logged as an error but not automatically queued for retry. This ensures our primary back-office operations are never blocked by external service outages.



## Programmatic Data Migration

Migrating from the legacy SQL database required extensive cleansing of unstructured text data.

Rather than relying entirely on raw SQL scripts, the migration is executed via a C# pipeline using `IEntitySeeder` implementations running against a `LegacyDbContext`. Custom parsers use Regex to extract and normalize phone numbers, emails, addresses, and timestamps from corrupted string fields.
Executing row-by-row normalization in memory takes longer than bulk SQL `INSERT INTO... SELECT` statements. However, this programmatic approach ensures complex relational linking (like resolving string names to newly generated Foreign Keys) and robust data cleansing without data loss. PostgreSQL sequences are manually resynced afterward to ensure auto-increments remain stable.

### Inline Value Objects vs. Dedicated Address Tables

The DBML schema initially proposed a normalized, standalone `addresses` table linked via foreign keys. However, the final implementation uses EF Core's `ComplexProperty` feature to store addresses inline as Value Objects within the `customers` and `parcels` tables.

Addresses are mapped as complex types, creating columns like `Address_City` in the Customer table and `Sender_City` / `Recipient_City` in the Parcel table.
This sacrifices strict normalization in favor of massive read-performance gains. It eliminates the need for expensive `JOIN` operations every time a parcel or customer profile is queried, which is crucial for a high-throughput logistics system.


### Cryptographic Hashing Standard

The DBML notes suggested updating the legacy plaintext passwords using `argon2id`.

The implementation utilizes `Rfc2898DeriveBytes.Pbkdf2` with a 256-bit SHA-256 hash algorithm, a 16-byte salt, and 100,000 iterations.
While Argon2id is the current industry gold standard for memory-hard password hashing, it requires pulling in external third-party libraries. PBKDF2 is natively supported by the .NET standard library.


### Immutable SLA Rule Versioning

To fulfill the requirement that SLA rules can be updated without breaking historical case data, the system treats SLA rules as immutable records.

When a user "updates" an SLA rule, the `UpdateSlaRuleCommandHandler` intercepts this, marks the existing `SlaRule` as `IsActive = false`, and inserts a brand new `SlaRule` record containing the updated parameters and `IsActive = true`.
This slightly increases the size of the `sla_rules` table over time as rules are modified. However, it guarantees that historical reporting on older cases—which were bound by the SLA rules active at their time of creation—remains 100% accurate and mathematically sound.


### Handling Legacy Data Anomaly Resolution in Memory

The legacy database contained highly unstructured data (e.g., dimensions stored as `"10x20x30 cm"`, booleans as `"yes"`, multiple tags stuffed into single strings).

We built robust C# parsing helpers (`StringParserHelper`, `AddressParserHelper`) that run during the EF Core seeding phase to extract integers, standard booleans, and cleanly split comma-separated arrays into actual many-to-many junction tables.
Running regex parsing and string manipulation in C# memory during migration takes longer than executing bulk SQL scripts. However, this ensures that no data is lost and that strict data types are perfectly adhered to when written to the new Postgres database.