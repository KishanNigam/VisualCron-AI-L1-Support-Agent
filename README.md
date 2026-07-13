# VisualCron AI L1 Support Agent

Enterprise-grade solution bootstrap for the VisualCron AI L1 Support Agent application.

## Overview

This repository contains the initial foundation for a Windows-based .NET 8 application intended to support future automation scenarios involving Outlook, AI assistance, GHC CLI integration, VisualCron workflows, batch and VBScript logs, archival, and enterprise logging.

## Solution Structure

- src/VisualCron.AI.L1.Agent/ - Main console application project
- VisualCron-AI-L1-Agent.sln - Solution file for Visual Studio Code and .NET tooling

## Requirements

- .NET SDK 8.0
- Visual Studio Code
- Windows environment

## Build

From the repository root:

```powershell
dotnet build VisualCron-AI-L1-Agent.sln
```

## Development Notes

The current implementation now includes foundational workflow capabilities such as Outlook mailbox validation, failure-mail subject parsing, failure-mail discovery from the Inbox, and processing-history persistence.

For AI-assisted continuation and handover context, see the documentation pack in [docs/AI_CONTEXT](docs/AI_CONTEXT).

## Enterprise Folder Structure

The repository now includes a clean-architecture-style structure for future growth:

- src/VisualCron.Agent/ - Entry-point and orchestration shell for automation workflows.
- src/VisualCron.Application/ - Application-layer coordination and use-case boundaries.
- src/VisualCron.Domain/ - Core domain concepts and business rules definitions.
- src/VisualCron.Infrastructure/ - External integrations and infrastructure concerns.
- src/VisualCron.Shared/ - Shared utilities and cross-cutting assets.
- src/VisualCron.Tests/ - Test project container for future automated validation.
- docs/architecture/ - Architecture documents and design guidance.
- docs/sprint/ - Sprint planning and delivery notes.
- docs/user-stories/ - User story tracking and requirements artifacts.
- docs/prompts/ - Prompt templates and workflow references.
- docs/decisions/ - Architecture decision records.
- runtime/archive/ - Archive storage for generated or retained content.
- runtime/logs/ - Runtime log output location.
- runtime/output/ - Output artifacts and generated files.
- runtime/prompts/ - Runtime prompt-related assets.
- runtime/temp/ - Temporary runtime files.
- runtime/reports/ - Reporting outputs.
- tools/ - Local developer utilities and supporting scripts.
- scripts/ - Operational and maintenance scripts.
- .github/ - GitHub workflow and repository automation files.
