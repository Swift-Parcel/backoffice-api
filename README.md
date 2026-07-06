# SwiftParcel Back-Office — Case Management System (C# Team)

## Project Overview

This is one half of the SwiftParcel internship project. Two teams are building two applications that must work together:

| Team | Application | Tech | Role |
|------|-------------|------|------|
| **C# Team (you)** | Back-Office API | C# / .NET | Backend for handlers, supervisors, admins. Owns the case & parcel database. |
| **Java Team** | Customer Portal API | Java / Spring Boot | Backend for senders & recipients. Owns customer accounts, pricing, pickup scheduling. |

Both applications are **REST APIs only** — no frontend work. A frontend may be built separately by mentors after your API design phase. Treat this like working with an off-shore frontend team: they will consume your APIs with minimal coordination, so your endpoints must be well-documented, self-explanatory, and return clear error messages.

**Both teams share a 3–4 week timeline.** You must agree on API contracts with the Java team early (ideally end of week 1) so integration work can proceed in parallel.

## Background

SwiftParcel is a mid-size parcel delivery company operating across Central Europe. They handle ~50,000 parcels/day. Their case management system handles customer complaints, delivery issues, lost/damaged parcels, and internal investigations.

The current system was migrated from a 15-year-old Microsoft Access application into a SQL database. The migration was done as a 1:1 copy — no restructuring, no normalization, no constraints. The system "works" but is plagued with data quality issues, slow queries, and makes it nearly impossible to build reliable reports or new features on top of it.

---

## Application Requirements

Requirements are split into **Required** (must be completed) and **Optional** (stretch goals if time permits).

### REQUIRED — Core Case Management

1. **Case Creation** — Customer service agents create cases when a customer calls/emails about an issue (lost parcel, damaged parcel, delayed delivery, wrong address, billing dispute).

2. **Case Assignment** — Cases are assigned to handlers based on case type and region. A case can be reassigned.

3. **Case Lifecycle** — Cases move through statuses: Open → In Progress → Awaiting Customer → Resolved → Closed. Cases can also be Escalated or Cancelled.

4. **Case Notes** — Handlers add timestamped notes to cases as they work on them. Notes can be internal (handler-only) or customer-visible.

5. **Parcel Tracking Link** — Each case is linked to one or more parcels. The system needs to know which parcel(s) a case is about.

6. **Customer Lookup** — Agents should be able to find all cases for a given customer (by email, phone, or customer ID).

### REQUIRED — Database Redesign

7. **Schema Redesign** — Analyze the legacy database (`init.sql`), identify all design problems, and redesign it into a properly normalized schema (3NF) with correct data types, foreign keys, and constraints. Write migration scripts to move the legacy data into the new schema. init.sql itself shouldn't be changed, you need to apply migrations to it without data loss.

### REQUIRED — Basic Roles & Authentication

8. **Role-Based Access Control** — The system has four roles:

    | Role | Access Level |
    |------|-------------|
    | **Read-Only** | Can view cases, notes, and parcels. Cannot create, edit, or change anything. Used by auditors and finance. |
    | **Operator** | Can create cases, add notes, change case status, and reassign cases within their own region. |
    | **Supervisor** | Everything an Operator can do, plus: access cases across all regions, manage escalations, view reports, manage handlers in their region. |
    | **Admin** | Full access to everything, including configuration, user management, and audit logs. |

9. **Region Scoping** — Operators see only their own region's cases. Supervisors can be granted multi-region access. Admins are always global.

### REQUIRED — Integration with Customer Portal (Java Team)

Your back-office application must expose REST APIs that the Java team's Customer Portal will consume. You must also notify their application when relevant events happen on your side.

**Capabilities you expose (Java team calls you):**

- Return current parcel status, location, and tracking history for a given tracking number.
- Return the estimated/scheduled delivery date and available time slots for a parcel.
- Create a new parcel record from a confirmed pickup request (return generated tracking number).
- Mark a parcel as delivery-confirmed when the recipient confirms receipt via the portal.
- Create a new case (complaint) submitted by a customer through the portal.
- Return the current status of a case and its resolution (customer-facing — must not expose internal notes or handler details).
- Accept a customer-visible note/message added to an existing case.
- Accept a satisfaction score (1–5) and optional comment after case resolution.
- Accept a delivery change request (creates a `DELIVERY_CHANGE` case for an operator to process).

**Events you notify the Java app about (you call them):**

- When a parcel status changes (picked up, in transit, out for delivery, delivered) — send tracking number and new status.
- When a case status changes (resolved, closed, etc.) — send case number, new status, and resolution text if applicable.
- When a `DELIVERY_CHANGE` case is resolved (approved/rejected) — send case number and outcome.

The Java team uses these notifications to update their local records and trigger customer notifications. If their service is unavailable, log the failure and continue — don't block your own operations.

**Integration ground rules:**
- Both teams must agree on the API contracts (request/response schemas, endpoint paths, authentication) by the end of **week 1**. You design the endpoints together.
- Use JSON over HTTP for all integration.
- Authentication between the two apps: use a shared API key in a request header (keep it simple).
- If the other team's service is unavailable, your app should handle it gracefully (return appropriate errors, don't crash).

### REQUIRED — Basic SLA

10. **SLA Deadlines** — Each case type has a configurable SLA deadline (in hours). When a case is created, the system calculates and stores the SLA deadline. Cases that exceed their SLA are flagged.

    | Case Type | Default SLA |
    |-----------|-------------|
    | LOST | 48h |
    | DAMAGED | 48h |
    | DELAYED | 48h |
    | WRONG_ADDRESS | 48h |
    | BILLING | 72h |
    | DELIVERY_CHANGE | 24h |
    | OTHER | 72h |

### REQUIRED — Basic Reporting

11. **Reports** — API endpoints providing:
    - Number of open cases by type
    - Number of SLA breaches (current and historical)
    - Average resolution time by case type
    - Cases per handler (workload overview)

---

### OPTIONAL — Redis Caching

12. **Caching with Redis** *(optional)* — Introduce Redis as a caching layer to improve read performance. Candidates for caching:
    - Case detail lookups (frequently accessed by handlers and the Java portal)
    - Parcel status/tracking data (high-traffic endpoint from the portal)
    - Reporting aggregations (expensive queries, results change slowly)
    - Handler workload counts (used by auto-assignment)

    Implement cache invalidation when underlying data changes (e.g., case status update should invalidate that case's cache entry). Consider TTL-based expiry for reporting data.

### OPTIONAL — Auto-Assignment & Escalation

13. **Auto-Assignment Rules** *(optional)* — When a case is created, the system suggests or auto-assigns a handler based on configurable rules. Rules consider: case type, region, handler workload, and handler skills. Rules have priorities — higher priority rules are evaluated first.

14. **Escalation Rules** *(optional)* — If a case is not resolved within its SLA window, the system automatically escalates it: reassign to a senior handler, notify a manager, or change the priority. Configurable per case type.

    *Microservice note: Auto-assignment and escalation are natural candidates for a separate "Routing & Escalation Service." This service would own the rules engine, listen for case-created and SLA-breach events, and issue assignment/escalation commands back to the main app. It could run on a schedule (polling) or react to events (message queue). Think about what interface this service would need.*

### OPTIONAL — Notifications & Templates

15. **Email Notifications** *(optional)* — Automated emails at lifecycle events: case created (to customer), case assigned (to handler), status changed (to customer), SLA warning (to handler), SLA breach (to handler + manager).

16. **Email Templates** *(optional)* — Templates with subject and body containing placeholders (`{{customer_name}}`, `{{case_number}}`). Templates exist per language (HU, DE, CZ, PL, EN) and can be overridden per region.

    *Microservice note: Notification sending is a classic candidate for extraction into a "Notification Service." The main app would publish events (case created, status changed, SLA breach) to a queue. The notification service would consume events, look up templates, resolve placeholders, and send emails. This decouples the case management logic from notification delivery, allows independent scaling, and makes it easy to add new channels (SMS, push) later without touching the main app.*

### OPTIONAL — Advanced SLA

17. **Business Hours SLA** *(optional)* — SLA calculated in business hours instead of calendar hours, accounting for regional working hours (e.g., Budapest: Mon-Fri 08:00–18:00) and public holidays.

### OPTIONAL — Advanced Configuration

18. **System Settings** *(optional)* — Configurable settings (default language, pagination, max attachment size, auto-close period, maintenance mode) manageable without code changes.

19. **Status Workflow Configuration** *(optional)* — Allowed status transitions are configurable (e.g., "Open" → "In Progress" or "Cancelled", but not directly "Open" → "Closed"). Certain transitions require a note (e.g., resolving requires a resolution note).

### OPTIONAL — Advanced Permissions

20. **Granular Permissions** *(optional)* — Beyond the four roles, individual permissions can be granted/revoked per user:
    - `case.delete`, `case.export`, `case.merge`, `config.sla.edit`, `config.template.edit`, `report.financial`, `customer.edit`

21. **Audit Trail** *(optional)* — All write actions are logged: who, when, what changed (old → new value). Append-only, immutable even for Admins.

    *Microservice note: Audit logging is a strong candidate for a separate "Audit Service." The main app emits structured audit events (who, what, when, old/new values) to a message queue. A dedicated audit service persists them in its own data store (potentially append-only, immutable). This ensures the audit trail can never be tampered with from the main app, and the write overhead doesn't slow down user-facing operations.*

---

## Business Rules

- A case must have exactly one primary handler at any time.
- Customers are identified by email (unique).
- Each parcel has a unique tracking number (format: `SP-XXXXXXXX`).
- Case types: LOST, DAMAGED, DELAYED, WRONG_ADDRESS, BILLING, DELIVERY_CHANGE, OTHER.
- VIP customers' cases are automatically bumped to high priority.
- Handlers have a maximum concurrent case limit; assignment must respect it.
- Cases created via the Customer Portal (Java app) should be automatically tagged with `channel: portal`.

## Current State

The database (`init.sql`) represents the current "as-is" state after the legacy migration. It has significant design problems:

- No foreign keys or referential integrity
- All fields stored as strings (including dates, numbers, booleans)
- Denormalized and redundant data
- Inconsistent data (same entities with different spellings/formats)
- Multiple values stored in single columns
- No constraints or validation at the DB level
- Mixed date formats
- Business logic encoded in string fields
- Configuration stored as key-value dumps and serialized blobs
- Workflow rules hardcoded in data
- Overlapping and contradictory rule definitions
- Users and handlers are separate, unlinked entities
- Permissions stored as comma-separated strings inside role rows
- Per-user permission overrides with no clear precedence over role permissions
- Plaintext passwords in the database
- Audit log with inconsistent action naming and orphaned references

## Your Task

**Phase 1 — Analysis & Planning (Week 1)**
- Identify all design problems in the current database
- Document data inconsistencies you find
- List which normal forms are violated and why
- **Agree on API contracts with the Java team** — define request/response schemas for all integration endpoints together
- Plan your new database schema

**Phase 2 — Redesign & Build (Weeks 2–3)**
- Build the normalized database schema (3NF) with proper types, FKs, constraints
- Write migration scripts for the legacy data
- Implement the core case management features (CRUD, lifecycle, notes, assignment)
- Implement the integration APIs so the Java team can start connecting
- Implement basic roles and authentication

**Phase 3 — Integration & Polish (Week 3–4)**
- End-to-end integration testing with the Java team
- Implement reporting
- Tackle optional features if time permits
- Fix bugs and edge cases discovered during integration

## Deliverables

1. Analysis document (database problems found, normal form violations)
2. New database schema DDL (tables, constraints, indexes)
3. Data migration scripts (legacy → new, including data cleaning)
4. Working back-office API with all required features
5. API documentation (clear enough for the FE team and Java team to consume without asking questions)
6. Integration with the Customer Portal (verified end-to-end)
7. Sample queries / endpoints for reporting requirements
8. Brief document of design decisions and trade-offs

## Notes

- Local infra setup is provided by mentors, if something is missing ask for it (redis, db etc.)
- Quantity and Quality of deliverables are hand-in-hand, but a good working application outweighs a missing documentation
- FE may be implemented by mentors after the design phase ends (as mentioned in the overview)