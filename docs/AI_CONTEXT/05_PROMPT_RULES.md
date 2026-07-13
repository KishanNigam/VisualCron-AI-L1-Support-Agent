# Prompt Rules

## Prompt Writing Rules
- Keep prompts concise, explicit, and structured.
- Avoid ambiguous language when describing workflow behavior.
- Ensure prompts reflect the current architecture and business rules.
- Never embed implementation-specific shortcuts that bypass the existing architecture.

## Coding Rules
- Follow Clean Architecture separation of concerns.
- Do not place business logic in the Agent host layer.
- Use interfaces for infrastructure boundaries.
- Preserve sequential processing behavior.
- Do not introduce AI or prompt generation logic unless explicitly requested by the approved backlog.

## Architecture Rules
- Keep the Agent layer as composition root only.
- Keep Application-layer use cases orchestration-focused.
- Keep Infrastructure implementations behind interfaces.
- Maintain configuration-driven behavior from appsettings.json.

## Testing Rules
- Write unit tests for parsing and discovery behavior.
- Favor deterministic, testable implementations.
- Do not test implementation details that are not user-visible behavior.

## Build Rules
- Ensure the solution builds successfully after every change.
- Preserve existing project structure and references.
- Validate runtime behavior where relevant.

## Review Rules
- Review changes for layering violations.
- Confirm that business rules remain centralized and testable.
- Reject architectural shortcuts that bypass the approved structure.
