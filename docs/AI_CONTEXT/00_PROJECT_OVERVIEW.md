# Project Overview

## Project Goal
VisualCron AI L1 Support Agent is an enterprise-grade .NET 8 Windows application designed to automate the L1 Production Support workflow for Outlook-driven failure handling.

## Business Problem
Operations teams receive failure emails from production systems and need a reliable, auditable, and repeatable process to:
- identify failure mail events,
- extract structured metadata,
- process attachments and metadata safely,
- generate acknowledgements and RCA drafts,
- archive execution history.

## Current Scope
The current implementation focuses on the enterprise bootstrap and the initial workflow foundation:
- Clean Architecture project structure
- Outlook mailbox connectivity and configuration
- Failure mail subject parsing
- Failure mail discovery from the Inbox
- Processing history persistence using local JSON files
- Sequential processing model and skip-on-repeat behavior

## Future Scope
Planned extensions include:
- reading and processing .log attachments,
- sending acknowledgement emails,
- building AI prompts,
- invoking GHC CLI automation,
- receiving and storing structured JSON results,
- generating and saving RCA drafts in Outlook,
- maintaining durable execution history.

## Technology Stack
- .NET 8
- C#
- Windows-focused desktop/server integration
- Outlook COM / MAPI integration
- Generic Host / Dependency Injection
- JSON-based local history storage
- xUnit for automated tests

## Architecture
The solution follows a Clean Architecture-inspired structure with clear separation between:
- Agent / Host layer
- Application layer
- Domain layer
- Infrastructure layer
- Shared layer
- Tests layer

## High Level Flow
1. Scan Outlook Inbox.
2. Filter failure mails using a configurable subject prefix.
3. Check processing history to determine whether the mail is new or already processed.
4. Parse subject metadata.
5. Continue with downstream processing workflows such as attachment handling and automation.
