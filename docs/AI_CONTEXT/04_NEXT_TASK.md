# Next Task

## Current Sprint
Sprint 4: Workflow orchestration and downstream processing integration

## Current User Story
US-005: Orchestrate processing for new failure mails using processing history as the source of truth

## Next Task
Implement the next processing stage for newly discovered failure mails while preserving sequential execution and history-driven behavior.

## Acceptance Criteria
- New failure mails are discovered from the Inbox.
- Processing history is checked before processing.
- Sequential processing is preserved.
- Missing or absent attachments do not block processing.
- Configuration remains the single source for runtime behavior.

## Definition of Done
- Application logic is implemented in the appropriate Application layer.
- Infrastructure implementations remain isolated behind interfaces.
- Automated tests cover the new behavior.
- Documentation remains aligned with the architecture.

## Test Cases
- New mail is processed and persisted to history.
- Repeat execution skips already processed mail.
- Missing attachment does not stop processing.
- Configurable prefix filters only matching failure mails.

## Expected Deliverables
- New application use case for processing discovered mails
- Additional infrastructure adapters for downstream workflow steps
- Updated tests and documentation
