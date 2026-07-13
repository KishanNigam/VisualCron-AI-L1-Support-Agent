# Architecture

## Architecture Diagram
```text
User / Operations
   |
   v
VisualCron.Agent
   |
   v
VisualCron.Application
   |        |
   |        +--> Use Cases / Services
   |
   +--> Domain Models / Contracts
   |
   v
VisualCron.Infrastructure
   |
   +--> Outlook integration
   +--> Failure mail discovery
   +--> Processing history repository
   +--> Logging

VisualCron.Shared
VisualCron.Tests
```

## Layer Responsibilities
### VisualCron.Agent
- Application host and startup composition
- Dependency injection wiring
- Configuration loading
- Runtime execution entry point

### VisualCron.Application
- Use cases and application abstractions
- Business orchestration contracts
- Strongly typed DTOs and models
- Domain-facing interfaces

### VisualCron.Domain
- Core domain concepts and business rules
- Domain models and shared business definitions

### VisualCron.Infrastructure
- Outlook mailbox interaction
- Failure mail discovery logic
- Processing history persistence
- Logging and runtime integrations

### VisualCron.Shared
- Shared cross-cutting helpers and common utilities

### VisualCron.Tests
- Unit tests for parsing, discovery, and related behavior

## Dependency Rules
- The Agent layer depends on Application and Infrastructure.
- Application depends on Domain and Shared abstractions.
- Infrastructure depends on Application and Domain abstractions.
- Infrastructure implementations are wired through DI.
- No business logic should be embedded in the Agent layer.

## Folder Structure
- src/VisualCron.Agent
- src/VisualCron.Application
- src/VisualCron.Domain
- src/VisualCron.Infrastructure
- src/VisualCron.Shared
- src/VisualCron.Tests

## Project References
- VisualCron.Agent references Application, Infrastructure, Shared
- VisualCron.Application references Domain, Shared
- VisualCron.Infrastructure references Application, Domain, Shared
- VisualCron.Tests references Application, Infrastructure, Agent
