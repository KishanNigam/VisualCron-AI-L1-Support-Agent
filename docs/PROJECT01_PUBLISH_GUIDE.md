# Project-01 Publish Guide
## VisualCron Prompt Generator

Version: 1.0

---

# Purpose

This document explains how to manually create the production executable for Project-01 (VisualCron Prompt Generator).

Output:

VisualCron.Agent.exe

---

# Prerequisites

Install:

- Visual Studio 2022
- .NET 8 SDK

Verify

```powershell
dotnet --version
```

Expected

```
8.x.x
```

---

# Project Location

Open the solution.

```
VisualCron-AI-L1-Agent.sln
```

or navigate to

```
src/
    VisualCron.Agent/
```

---

# Build Verification

Run

```powershell
dotnet build
```

Expected

```
0 Error

0 Warning
```

Do NOT publish if build fails.

---

# Publish Command

Run from repository root.

```powershell
dotnet publish src/VisualCron.Agent/VisualCron.Agent.csproj `
-c Release `
-r win-x64 `
--self-contained true `
-p:PublishSingleFile=true `
-p:PublishReadyToRun=true `
-p:PublishTrimmed=false
```

---

# Publish Output

Output Folder

```
src/

    VisualCron.Agent/

        bin/

            Release/

                net8.0/

                    win-x64/

                        publish/
```

---

# Required Publish Files

```
publish/

    VisualCron.Agent.exe

    appsettings.json

    appsettings.Development.json

    runtime/
```

runtime folder should contain

```
runtime/

    archive/

        history/

    executions/

    logs/
```

---

# Standalone Testing

Create

```
D:\PromptGenerator_Test\
```

Copy

```
VisualCron.Agent.exe

appsettings.json

runtime/
```

Create BAT

```
@echo off

cd /d "%~dp0"

VisualCron.Agent.exe

pause
```

Run BAT.

---

# Expected Workflow

```
Outlook

↓

Failure Mail Discovery

↓

Attachment Download

↓

Log Reader

↓

Execution Workspace

↓

Prompt.md

↓

Application Exit
```

---

# Success Criteria

Verify

✓ Application starts

✓ Outlook connects

✓ Failure mail discovered

✓ Attachment downloaded

✓ Log files read

✓ Execution Workspace created

✓ Prompt.md generated

✓ Application exits successfully

---

# Publish Checklist

Before every release

□ Build Success

□ Publish Success

□ EXE Generated

□ Standalone Test Passed

□ Prompt.md Generated

□ Duplicate Detection Passed

---

# Notes

Project-01 responsibility ends after Prompt.md generation.

No AI processing is performed.

No Draft Mail generation is performed.

This executable is responsible ONLY for generating Prompt.md.