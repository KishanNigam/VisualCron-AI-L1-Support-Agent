# 03_FILE_STRUCTURE.md

## Purpose

This document lists the important files in the current repository and explains their responsibilities, inputs, outputs, and dependencies.

---

## 1. Root-level files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| Directory.Build.props | Shared MSBuild settings for all projects | Project build configuration | Consistent build properties | MSBuild / dotnet |
| global.json | Pins the .NET SDK version | SDK version | Build toolchain selection | dotnet SDK |
| README.md | Project overview and build instructions | Repository context | Documentation for developers | None |
| RunCopilot.ps1 | Wrapper for running the Copilot-related workflow | Script parameters and environment | Execution of the Copilot runner | PowerShell, repository files |
| VisualCron-AI-L1-Agent.sln | Solution entry point | Project list | Build and test orchestration | All projects |

---

## 2. Entry-point and bootstrapping files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.Agent/Program.cs | Main runtime entry point | Command-line args, configuration, dependencies | Hosted workflow execution, logs, prompt generation | Host builder, DI, use cases, parser, logger |
| src/VisualCron.Agent/Extensions/ApplicationServiceCollectionExtensions.cs | Registers application-layer services | IServiceCollection | Use-case registrations | Application use case types |
| src/VisualCron.Agent/Extensions/InfrastructureServiceCollectionExtensions.cs | Registers infrastructure-layer implementations | IServiceCollection, IConfiguration | Concrete infrastructure services and options binding | Outlook options, infrastructure classes |
| src/VisualCron.Agent/Extensions/SharedServiceCollectionExtensions.cs | Placeholder for shared services | IServiceCollection | Empty service registration extension | None |
| src/VisualCron.Agent/appsettings.json | Runtime configuration for the host | JSON configuration | Binding to options classes | Configuration system |
| src/VisualCron.Agent/appsettings.Development.json | Development override settings | JSON configuration | Environment-specific settings | Configuration system |
| src/VisualCron.Agent/VisualCron.Agent.csproj | Console app project definition | Project references, package references | Buildable executable | .NET SDK, referenced projects |

---

## 3. AI entry-point project

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.AI.L1.Agent/Program.cs | Placeholder entry point for a future enterprise AI implementation | None currently | No runtime behavior yet | None |
| src/VisualCron.AI.L1.Agent/VisualCron.AI.L1.Agent.csproj | Project definition for future AI implementation | Project references | Buildable project shell | .NET SDK |

---

## 4. Application-layer files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.Application/Outlook/DiscoverFailureMailsUseCase.cs | Orchestrates failure-mail discovery, filtering, and history tracking | Execution ID, discovery service, repository, logger | FailureMailDiscoveryResult | IFailureMailDiscoveryService, IProcessingHistoryRepository |
| src/VisualCron.Application/Outlook/DownloadFailureMailAttachmentsUseCase.cs | Wraps attachment download behavior | Mail item object | List of downloaded attachments | IFailureMailAttachmentDownloader |
| src/VisualCron.Application/Outlook/ReadUnreadEmailsUseCase.cs | Reads unread email DTOs | Cancellation token | IReadOnlyList<UnreadEmailDto> | IUnreadEmailReader |
| src/VisualCron.Application/Outlook/ReadLogFilesUseCase.cs | Wraps log-file reading logic | File paths | IReadOnlyList<LogFileReadResult> | ILogFileReader |
| src/VisualCron.Application/Outlook/OutlookOptions.cs | Binds Outlook settings from configuration | Configuration values | Strongly typed options | IConfiguration binding |
| src/VisualCron.Application/Outlook/FailureMailDiscoveryItem.cs | Represents a discovered failure mail | Mail metadata and mail item | Structured discovery item | None |
| src/VisualCron.Application/Outlook/FailureMailDiscoveryResult.cs | Carries the result of mail discovery | Discovery counts and mail list | Structured result object | FailureMailDiscoveryItem |
| src/VisualCron.Application/Outlook/FailureMailMetadata.cs | Represents parsed subject metadata | Subject string | Parsed fields such as job/environment/server | None |
| src/VisualCron.Application/Outlook/LogFileReadResult.cs | Represents the result of reading a log file | Log path and content | Parsed log metadata and content | None |
| src/VisualCron.Application/Outlook/ProcessedEmailRecord.cs | Represents persisted processing history | Incident information and timestamps | JSON-serializable record | None |
| src/VisualCron.Application/Outlook/DownloadedAttachment.cs | Represents a downloaded attachment | File name, path, metadata | Attachment info for downstream use | None |
| src/VisualCron.Application/Outlook/UnreadEmailDto.cs | Represents an unread email item | Mail metadata | DTO used by the application | None |
| src/VisualCron.Application/Outlook/IFailureMailDiscoveryService.cs | Declares the discovery contract | None | Discovery contract | None |
| src/VisualCron.Application/Outlook/IFailureMailAttachmentDownloader.cs | Declares the attachment-download contract | Mail item | Download contract | None |
| src/VisualCron.Application/Outlook/IProcessingHistoryRepository.cs | Declares the processing-history persistence contract | Incident key and record | Persistence contract | None |
| src/VisualCron.Application/Outlook/IUnreadEmailReader.cs | Declares the unread-email reader contract | None | Reading contract | None |
| src/VisualCron.Application/Outlook/ILogFileReader.cs | Declares the log-reader contract | File paths | Reading contract | None |
| src/VisualCron.Application/Outlook/IFailureMailSubjectParser.cs | Declares the parser contract | Subject string | Parsing contract | None |
| src/VisualCron.Application/Outlook/IDownloadFailureMailAttachmentsUseCase.cs | Declares the attachment-download use case contract | Mail item | Use-case contract | None |
| src/VisualCron.Application/Class1.cs | Placeholder class | None | Placeholder | None |

---

## 5. Infrastructure-layer files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.Infrastructure/Outlook/OutlookMailboxService.cs | Validates Outlook availability and mailbox access | Options, logger | Outlook COM object | Outlook COM API |
| src/VisualCron.Infrastructure/Outlook/UnreadEmailReader.cs | Reads unread inbox messages | Outlook mailbox object | List of unread email DTOs | Outlook mailbox service |
| src/VisualCron.Infrastructure/Outlook/FailureMailDiscoveryService.cs | Scans the inbox for failure-related mails | Outlook mailbox object, options, logger | List of discovered failure mail items | Outlook mailbox service |
| src/VisualCron.Infrastructure/Outlook/FailureMailAttachmentDownloader.cs | Saves .log attachments to disk | Mail item object, logger | List of downloaded attachments | Outlook COM API, file system |
| src/VisualCron.Infrastructure/Outlook/LogFileReader.cs | Reads .log files from disk | File paths | Log contents and metadata | File system |
| src/VisualCron.Infrastructure/Outlook/JsonProcessingHistoryRepository.cs | Persists processing history as JSON | ProcessedEmailRecord | JSON files in archive/history | File system |
| src/VisualCron.Infrastructure/Outlook/FailureMailSubjectParser.cs | Parses subject text into metadata | Raw subject | FailureMailMetadata | Regex parser |
| src/VisualCron.Infrastructure/Class1.cs | Placeholder class | None | Placeholder | None |

---

## 6. Domain and shared placeholder files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.Domain/Class1.cs | Placeholder domain file | None | Placeholder | None |
| src/VisualCron.Shared/Class1.cs | Placeholder shared utility file | None | Placeholder | None |

---

## 7. Test files

| File | Responsibility | Input | Output | Dependencies |
|---|---|---|---|---|
| src/VisualCron.Tests/FailureMailAttachmentDownloaderTests.cs | Tests attachment download behavior | Mail/attachment scenarios | Test assertions | Infrastructure implementation |
| src/VisualCron.Tests/FailureMailDiscoveryUseCaseTests.cs | Tests discovery use case logic | Discovery scenarios | Test assertions | Application use case |
| src/VisualCron.Tests/FailureMailSubjectParserTests.cs | Tests subject parsing logic | Subject strings | Test assertions | Parser |
| src/VisualCron.Tests/LogFileReaderTests.cs | Tests log file reading behavior | Log file content | Test assertions | Log reader |
| src/VisualCron.Tests/UnitTest1.cs | Placeholder test | None | Placeholder | None |
| src/VisualCron.Tests/VisualCron.Tests.csproj | Test project definition | Test references | Executable test project | .NET SDK, test dependencies |
