# AI Handover

## Project Summary
VisualCron AI L1 Support Agent is an enterprise-grade .NET 8 Windows application intended to automate the L1 Production Support workflow for Outlook-triggered failure handling.

## Current Progress
The solution currently includes:
- Clean Architecture project structure
- Host startup and DI wiring
- Outlook mailbox validation
- Failure mail subject parsing
- Failure mail discovery from Inbox
- Processing history persistence using local JSON files

## Architecture
Follow the existing Clean Architecture layering:
- Agent for host composition
- Application for use cases and orchestration
- Domain for core business concepts
- Infrastructure for Outlook and persistence integrations
- Shared for common helpers
- Tests for validation

## Coding Rules
- Do not recreate existing projects.
- Keep business logic in Application-layer use cases.
- Preserve interfaces and infrastructure abstraction boundaries.
- Use configuration from appsettings.json instead of hardcoding runtime behavior.
- Preserve sequential processing semantics.

## Business Rules
- Unread mail is not the source of truth.
- The application scans the Inbox.
- Failure mails are discovered by a configurable subject prefix.
- Processing history is the authoritative source for duplicate handling.
- Missing attachments must not block processing.
- The application must not depend on Outlook read/unread state.

## Current Sprint
Sprint 4: Workflow orchestration and downstream processing integration

## Current User Story
US-005: Orchestrate processing for new failure mails using processing history as the source of truth

## Current Task
Implement the next downstream processing stage for newly discovered failure mails while preserving the current architecture.

## Testing Rules
- Add or update unit tests for any new behavior.
- Keep tests deterministic and focused on outcomes.
- Validate the build after changes.

## How to Continue Development
1. Review the architecture documents in docs/AI_CONTEXT.
2. Preserve the current project structure and layering.
3. Extend the application layer with new orchestration use cases.
4. Implement new infrastructure capabilities behind interfaces.
5. Keep history-driven behavior and sequential processing intact.
6. Validate with automated tests and runtime verification.
