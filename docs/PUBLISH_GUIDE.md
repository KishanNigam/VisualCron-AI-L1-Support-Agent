# Publish Guide

## Publish command

Run the following from the repository root:

```powershell
dotnet publish src/VisualCron.Agent/VisualCron.Agent.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false
```

## Output folder

The published output is created in:

- src/VisualCron.Agent/bin/Release/net8.0/win-x64/publish/

## Required files beside the EXE

The publish output is a single-file executable, so the main runtime artifact is:

- VisualCron.Agent.exe

The application also expects the following runtime resources to be available in the publish folder:

- appsettings.json
- appsettings.Development.json

## Fresh machine setup

1. Install the Microsoft .NET 8 runtime or use the self-contained publish output.
2. Copy the entire publish folder to the target machine.
3. Ensure the target machine has access to Outlook and the required mailbox configuration.
4. Run VisualCron.Agent.exe from the publish folder.

## How to execute manually

From the publish folder, run:

```powershell
./VisualCron.Agent.exe
```
