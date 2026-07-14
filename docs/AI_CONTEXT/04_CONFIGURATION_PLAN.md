# 04_CONFIGURATION_PLAN.md

## Purpose

This document identifies the hardcoded values present in the current implementation and proposes configuration keys, default values, and future storage locations for them.

---

## 1. Current hardcoded values and proposed configuration plan

| Current hardcoded value | Current source | Proposed configuration key | Default value | Future appsettings.json location |
|---|---|---|---|---|
| Outlook mailbox name in config file | src/VisualCron.Agent/appsettings.json | Outlook:Mailbox | riskon2819@gmail.com | Same directory as the single EXE, under appsettings.json |
| Maximum emails scanned | src/VisualCron.Infrastructure/Outlook/FailureMailDiscoveryService.cs and OutlookOptions | Outlook:MaxEmailsToScan | 50 | Same directory as the single EXE |
| Failure mail subject prefix | src/VisualCron.Agent/appsettings.json | Outlook:FailureMailSubjectPrefix | [EXTERNAL] EAS-P5-MW | Same directory as the single EXE |
| Outlook COM ProgID | src/VisualCron.Infrastructure/Outlook/OutlookMailboxService.cs | Outlook:ComProgId | Outlook.Application | Same directory as the single EXE |
| Outlook namespace | src/VisualCron.Infrastructure/Outlook/OutlookMailboxService.cs and FailureMailDiscoveryService.cs | Outlook:Namespace | MAPI | Same directory as the single EXE |
| Default Inbox folder id | src/VisualCron.Infrastructure/Outlook/UnreadEmailReader.cs and FailureMailDiscoveryService.cs | Outlook:InboxFolderId | 6 | Same directory as the single EXE |
| Runtime temp folder | src/VisualCron.Infrastructure/Outlook/FailureMailAttachmentDownloader.cs | Runtime:TempPath | runtime/temp | Same directory as the single EXE |
| Runtime archive/history folder | src/VisualCron.Infrastructure/Outlook/JsonProcessingHistoryRepository.cs | Archive:HistoryPath | runtime/archive/history | Same directory as the single EXE |
| Prompt file name | src/VisualCron.Agent/Program.cs | Runtime:PromptFileName | Prompt.md | Same directory as the single EXE |
| Prompt folder location | src/VisualCron.Agent/Program.cs | Runtime:PromptPath | runtime/prompts | Same directory as the single EXE |
| AI output folder | not implemented yet | AI:OutputPath | runtime/output/ai | Same directory as the single EXE |
| Archive folder root | appsettings.json currently has Archive:Path = Placeholder | Archive:Path | runtime/archive | Same directory as the single EXE |
| Scripts folder | repository root contains scripts/ | Runtime:ScriptsPath | scripts | Same directory as the single EXE |
| Log folder | runtime/logs exists in repository | Logging:LogPath | runtime/logs | Same directory as the single EXE |
| Copilot runner script | repository root contains RunCopilot.ps1 | Copilot:RunnerScript | RunCopilot.ps1 | Same directory as the single EXE |
| Copilot working directory | current script logic depends on repository context | Copilot:WorkingDirectory | . | Same directory as the single EXE |
| AI provider name | appsettings.json currently Placeholder | AI:Provider | Placeholder | Same directory as the single EXE |
| AI model | appsettings.json currently Placeholder | AI:Model | Placeholder | Same directory as the single EXE |
| Runtime workspace path | appsettings.json currently Placeholder | Runtime:WorkspacePath | runtime | Same directory as the single EXE |
| Application environment | appsettings.Development.json | Application:Environment | Development | Same directory as the single EXE |

---

## 2. Recommended configuration structure

```json
{
  "Application": {
    "Name": "VisualCron AI L1 Support Agent",
    "Environment": "Development"
  },
  "Outlook": {
    "Mailbox": "riskon2819@gmail.com",
    "MaxEmailsToScan": 50,
    "FailureMailSubjectPrefix": "[EXTERNAL] EAS-P5-MW",
    "ComProgId": "Outlook.Application",
    "Namespace": "MAPI",
    "InboxFolderId": 6
  },
  "Runtime": {
    "BasePath": "runtime",
    "TempPath": "runtime/temp",
    "PromptPath": "runtime/prompts",
    "PromptFileName": "Prompt.md",
    "WorkspacePath": "runtime",
    "ScriptsPath": "scripts"
  },
  "Archive": {
    "Path": "runtime/archive",
    "HistoryPath": "runtime/archive/history"
  },
  "AI": {
    "Provider": "Placeholder",
    "Model": "Placeholder",
    "OutputPath": "runtime/output/ai"
  },
  "Logging": {
    "Level": "Information",
    "LogPath": "runtime/logs"
  },
  "Copilot": {
    "RunnerScript": "RunCopilot.ps1",
    "WorkingDirectory": "."
  }
}
```

---

## 3. Configuration ownership plan

### Mailbox

- owned by Outlook:Mailbox
- should be overrideable per environment
- should be validated at startup

### Runtime folder

- owned by Runtime:BasePath
- should define the shared root for temp, prompts, output, archive, and logs

### Prompt folder

- owned by Runtime:PromptPath
- should receive generated Prompt.md files

### AI output folder

- owned by AI:OutputPath
- should receive future AI-generated reports and result files

### Archive folder

- owned by Archive:Path or Archive:HistoryPath
- should contain persisted history and retained artifacts

### Scripts folder

- owned by Runtime:ScriptsPath or Scripts:Path
- should contain support scripts for automation and local workflow execution

### Log folder

- owned by Logging:LogPath
- should receive runtime log files once the logging strategy is expanded

### Copilot runner

- owned by Copilot:RunnerScript
- should point to the runner script used for AI or Copilot-based execution

---

## 4. Future appsettings.json location

For a single-EXE deployment, the most practical location is:

```text
<executable directory>/appsettings.json
```

This keeps configuration local to the deployed application and avoids depending on the source tree layout. A production deployment should also support:

- optional environment-specific overrides such as appsettings.Production.json
- a writable runtime directory next to the EXE
- an override path for external configuration if necessary
