# Actors

## Business Description
This section identifies the human and system actors that interact with the VisualCron AI L1 Support Agent workflow.

## Actors

### ACT-001: Support Analyst
- **Description:** Reviews inbound incidents, validates results, and oversees escalations.
- **Responsibilities:** Review email intake, review generated draft RCA, and approve or reject outputs.
- **Priority:** High
- **Dependencies:** Workflow outputs and acknowledgement handling
- **Future Notes:** Role-based approvals may be introduced.

### ACT-002: Operations Engineer
- **Description:** Oversees workflow execution, operational health, and incident follow-up.
- **Responsibilities:** Monitor execution history, investigate failures, and coordinate remediation.
- **Priority:** High
- **Dependencies:** Execution records and operational reporting
- **Future Notes:** Additional monitoring capabilities may be added.

### ACT-003: Outlook Mail System
- **Description:** Provides inbound incident emails and attachments.
- **Responsibilities:** Deliver incident messages to the workflow.
- **Priority:** High
- **Dependencies:** Mail integration availability
- **Future Notes:** Additional mail source connectors may be introduced.

### ACT-004: GHC CLI
- **Description:** Executes external automation or command-based tasks required by the workflow.
- **Responsibilities:** Perform required automation invocation operations.
- **Priority:** High
- **Dependencies:** CLI availability and command definition
- **Future Notes:** Additional execution modes may be added.

### ACT-005: AI Processing Service
- **Description:** Receives structured prompts and returns analysis results.
- **Responsibilities:** Produce structured output based on the provided input context.
- **Priority:** High
- **Dependencies:** Prompt design and response schema
- **Future Notes:** Additional model providers may be supported.
