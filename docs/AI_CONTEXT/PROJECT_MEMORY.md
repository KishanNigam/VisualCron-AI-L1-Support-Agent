# Project Memory

## Business Decisions
- Unread mail is not the source of truth.
- The application scans the Inbox and discovers failure mails directly.
- Processing history is the authoritative tracking mechanism for duplicates and repeat processing.
- Processing must remain sequential.
- Missing attachments must not stop processing.
- The application must not depend on Outlook read/unread state.
- Runtime behavior should be configurable from appsettings.json.

## Architecture Decisions
- The solution uses a Clean Architecture-inspired project structure.
- The Agent layer is the host composition root.
- Application-layer use cases orchestrate behavior.
- Infrastructure implementations are isolated behind interfaces.
- Processing history is stored locally as JSON files for the current implementation.

## Approved Change Requests
- CR-001: Unread mail replaced by processing history.
  - Status: Approved
  - Reason: Enterprise reliability and repeatable execution
- CR-002: Failure mail discovery based on configurable subject prefix.
  - Status: Approved
  - Reason: Flexible mailbox filtering

## Lessons Learned
- Runtime configuration loading must be validated carefully for host execution.
- Strongly typed options binding significantly improves maintainability.
- A repository abstraction is required for reliable duplicate handling.
- Local JSON persistence is a suitable starting point for history tracking.

## Known Technical Debt
- History storage is currently local JSON and not yet database-backed.
- Outlook integration remains tightly coupled to the Windows environment.
- Attachment processing is not yet implemented.

## Bug History
- Configuration binding issue for Outlook mailbox settings was resolved.
- Runtime content root issue for appsettings.json loading was resolved.

## Completed Features
- Solution bootstrap and project scaffolding
- Outlook mailbox validation
- Failure mail subject parsing
- Failure mail discovery from Inbox
- Processing history persistence and duplicate detection

## Pending Features
- Attachment reading and processing
- Acknowledgement email sending
- AI prompt generation
- GHC CLI execution
- RCA draft generation and Outlook saving
- Full execution archive and reporting

## Important Notes
- Future development should preserve the current architectural boundaries.
- New features should be introduced through interfaces and application use cases.
- Any automation step added later should remain configurable and testable.
