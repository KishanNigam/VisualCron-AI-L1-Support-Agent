# 05_DEPLOYMENT_PLAN.md

## Purpose

This document describes a deployment plan for the current project as a single EXE application. It is based on the current implementation and identifies which files remain external, which are generated at runtime, and which are configuration-driven.

---

## 1. Target deployment model

Target: single EXE deployment for a Windows environment.

### Deployment goal

Package the runtime so that the application can run from one executable with minimal external dependencies, while still allowing:

- Outlook access
- runtime output generation
- configuration file loading
- local history and prompt persistence

---

## 2. Files that remain external

These files or dependencies should remain outside the executable bundle and be available at runtime:

| Category | Item | Notes |
|---|---|---|
| Configuration | appsettings.json | Required for mailbox, runtime paths, and other settings |
| Configuration | appsettings.Development.json | Optional environment-specific override |
| Runtime dependency | Outlook desktop application | Required because the solution uses Outlook COM automation |
| Runtime dependency | Outlook mailbox profile | Required to locate the configured mailbox |
| Runtime directory | runtime/ | Needed for prompts, temp files, logs, and archive output |
| Scripts | scripts/ | Needed if the workflow uses helper scripts or the Copilot runner |
| Runner | RunCopilot.ps1 | Needed if Copilot execution is invoked by the workflow |

### Why these remain external

- the application currently depends on the local Windows desktop Outlook installation
- runtime artifact folders must remain writable
- configuration should be editable without rebuilding the executable

---

## 3. Files generated at runtime

These items are produced during execution and should be placed in a writable runtime folder:

| Generated item | Default location | Purpose |
|---|---|---|
| Temporary downloaded attachments | runtime/temp/<timestamp>/ | Stores .log files downloaded from email attachments |
| Processing history JSON files | runtime/archive/history/ | Prevents duplicate processing of the same incident |
| Prompt files | runtime/prompts/ or next to the log file | Stores the AI prompt content |
| AI output files | runtime/output/ai/ | Future location for AI-generated reports |
| Log files | runtime/logs/ | Future location for structured runtime logging |

### Runtime artifact behavior

- attachment download creates a timestamped execution folder
- processing history is stored as JSON per incident
- Prompt.md is generated for each analyzed incident

---

## 4. Files that are configuration

These are configuration inputs rather than executable content:

| Configuration file | Purpose |
|---|---|
| appsettings.json | Main configuration file for mailbox, runtime paths, and feature flags |
| appsettings.Development.json | Development-specific overrides |
| Optional environment-specific appsettings files | Future production / test separation |

### Configuration values expected in the deployed app

- Outlook mailbox
- mail subject prefix
- number of emails to scan
- runtime path roots
- AI provider metadata
- Copilot runner details

---

## 5. Recommended folder layout for deployment

```text
<deploy root>/
├── VisualCron.Agent.exe
├── appsettings.json
├── appsettings.Development.json   (optional)
├── runtime/
│   ├── archive/
│   ├── logs/
│   ├── output/
│   ├── prompts/
│   └── temp/
├── scripts/
└── RunCopilot.ps1                  (optional, if used)
```

---

## 6. Deployment notes

### Single EXE packaging

The application is already structured as a .NET console application and can be published as a standalone executable for Windows. The deployment model should preserve the runtime directory as a writeable folder.

### Important deployment assumptions

- the EXE must run in an environment that has Outlook installed and configured
- the deployment folder must be writable for runtime artifacts
- configuration should be adjusted after deployment rather than baked into the build

### Operational considerations

- runtime files should be isolated from the executable binaries
- prompt and output folders should be created automatically if absent
- history should not be stored inside the EXE payload

---

## 7. Summary

For a single-EXE deployment, the executable should remain small and self-contained, while the following remain external or generated:

- external: configuration file, Outlook desktop dependency, mailbox profile, scripts, runner script
- generated: temp downloads, prompt files, AI output, history JSONs, logs
- configuration: mailbox settings, runtime folder paths, AI provider metadata, Copilot runner path
