[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$PromptPath,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Write-Log {
    param([string]$Message)

    Write-Host $Message
}

if ([string]::IsNullOrWhiteSpace($PromptPath)) {
    Write-Log 'Prompt Path is required.'
    exit 1
}

$resolvedPromptPath = [System.IO.Path]::GetFullPath($PromptPath)

if (-not (Test-Path -LiteralPath $resolvedPromptPath -PathType Leaf)) {
    Write-Log 'Prompt Path not found.'
    exit 1
}

$promptContent = Get-Content -LiteralPath $resolvedPromptPath -Raw
$promptSize = $promptContent.Length
Write-Log "Prompt Path: $resolvedPromptPath"
Write-Log "Prompt Size: $promptSize bytes"

if ([string]::IsNullOrWhiteSpace($promptContent)) {
    Write-Log 'Prompt is empty.'
    exit 2
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    Write-Log 'Output Path is required.'
    exit 4
}

$resolvedOutputPath = [System.IO.Path]::GetFullPath($OutputPath)

$copilotCommand = Get-Command copilot -ErrorAction SilentlyContinue
if ($null -eq $copilotCommand) {
    Write-Log 'Copilot command not found.'
    exit 3
}

Write-Log 'Executing Copilot'
Write-Log 'Waiting'

try {
    Set-StrictMode -Off
    $copilotOutput = Get-Content -LiteralPath $resolvedPromptPath -Raw | & $copilotCommand.Source --allow-all --silent 2>&1
    $exitCode = $LASTEXITCODE
}
catch {
    Write-Log "Copilot execution failed: $($_.Exception.Message)"
    exit 3
}
finally {
    Set-StrictMode -Version Latest
}

if ($exitCode -ne 0) {
    Write-Log "Copilot execution failed with exit code $exitCode."
    exit 3
}

$outputDirectory = Split-Path -Parent $resolvedOutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$copilotOutput | Set-Content -LiteralPath $resolvedOutputPath -Encoding utf8

if (-not (Test-Path -LiteralPath $resolvedOutputPath -PathType Leaf)) {
    Write-Log 'Output file was not created.'
    exit 4
}

$outputItem = Get-Item -LiteralPath $resolvedOutputPath
$outputSize = $outputItem.Length
Write-Log "Output Path: $resolvedOutputPath"
Write-Log "Output Created"
Write-Log "Output Size: $outputSize bytes"

if ($outputSize -le 0) {
    Write-Log 'Output file is empty.'
    exit 5
}

Write-Log 'Completed'
exit 0
