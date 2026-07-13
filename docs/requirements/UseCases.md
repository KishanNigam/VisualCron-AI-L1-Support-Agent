# Use Cases

## Business Description
This section describes the primary use cases for the VisualCron AI L1 Support Agent workflow.

## Use Cases

### UC-001: Process Incoming Incident Email
- **Description:** The system receives and evaluates an incoming incident email.
- **Primary Actor:** Support Analyst
- **Acceptance Criteria:** The workflow identifies the incident and prepares it for further processing.
- **Priority:** High
- **Dependencies:** Email intake and subject validation
- **Future Notes:** Additional routing rules may be supported later.

### UC-002: Collect Supporting Evidence
- **Description:** The system gathers attachments and logs required for the incident review.
- **Primary Actor:** Operations Engineer
- **Acceptance Criteria:** Relevant evidence is collected and associated with the incident.
- **Priority:** High
- **Dependencies:** Log collection and attachment handling
- **Future Notes:** Additional evidence sources may be added.

### UC-003: Generate AI Prompt
- **Description:** The system constructs a structured prompt based on the incident evidence.
- **Primary Actor:** AI Processing Service
- **Acceptance Criteria:** A valid prompt is created and passed to downstream processing.
- **Priority:** High
- **Dependencies:** Prompt construction logic
- **Future Notes:** Prompt templates may be refined.

### UC-004: Execute External Automation
- **Description:** The system triggers required command execution through the GHC CLI.
- **Primary Actor:** GHC CLI
- **Acceptance Criteria:** The required external process is initiated successfully.
- **Priority:** High
- **Dependencies:** Execution environment and command definition
- **Future Notes:** More automation commands may be supported.

### UC-005: Produce RCA Draft
- **Description:** The system creates a draft RCA artifact from the processed incident information.
- **Primary Actor:** Support Analyst
- **Acceptance Criteria:** A draft RCA is produced for review.
- **Priority:** Medium
- **Dependencies:** Structured response and evidence assembly
- **Future Notes:** Review-state and approval flows may be added.

### UC-006: Send Acknowledgement
- **Description:** The system sends a response acknowledging receipt of the incident.
- **Primary Actor:** Outlook Mail System
- **Acceptance Criteria:** The acknowledgement is sent according to workflow rules.
- **Priority:** High
- **Dependencies:** Mail dispatch capability
- **Future Notes:** Message templates may be extended.
