#!/usr/bin/env pwsh
#Requires -Version 7.0

<#
.SYNOPSIS
    Publishes LanSweeper.Api package to NuGet.org

.DESCRIPTION
    This script performs the following steps, stopping if any fails:
    1. Checks git is porcelain (clean working directory)
    2. Determines the Nerdbank git version
    3. Checks nuget-key.txt exists, has content, and is gitignored
    4. Runs unit tests (unless -SkipTests is specified)
    5. Publishes to nuget.org

.PARAMETER SkipTests
    Skip running tests

.EXAMPLE
    .\Publish.ps1
    Standard publish workflow

.EXAMPLE
    .\Publish.ps1 -SkipTests
    Publish without running tests

.NOTES
    Requires:
    - PowerShell 7.0+
    - .NET SDK
    - nuget-key.txt file in solution root (must be gitignored)

    Exit codes:
    0 - Success
    1 - Git not porcelain
    2 - Version detection failed
    3 - nuget-key.txt missing, empty, or not gitignored
    4 - Tests failed
    5 - Build/pack failed
    6 - Publish failed
#>

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$SkipTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

function Write-Status {
    param([string]$Message)
    Write-Host "[STATUS] $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Step 1: Check git is porcelain
Write-Status "Checking git status..."
$gitStatus = git status --porcelain 2>&1
if ($gitStatus) {
    Write-Error "Git working directory is not clean"
    git status --short
    exit 1
}
Write-Success "Git working directory is clean"

# Step 2: Determine Nerdbank git version
Write-Status "Determining version..."

# Build once to generate version info, then extract from assembly
dotnet build LanSweeper.Api/LanSweeper.Api.csproj --configuration Release --verbosity quiet 2>&1 | Out-Null

# Get version from the built assembly
$assemblyPath = "LanSweeper.Api/bin/Release/net10.0/LanSweeper.Api.dll"
if (-not (Test-Path $assemblyPath)) {
    Write-Error "Could not find built assembly to determine version"
    exit 2
}

$assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName((Resolve-Path $assemblyPath))
$version = $assemblyName.Version.ToString(3)

if ([string]::IsNullOrWhiteSpace($version)) {
    Write-Error "Could not determine package version"
    exit 2
}
Write-Success "Version: $version"

# Step 3: Check nuget-key.txt exists, has content, and is gitignored
Write-Status "Checking nuget-key.txt..."

if (-not (Test-Path "nuget-key.txt")) {
    Write-Error "nuget-key.txt not found"
    exit 3
}

$apiKey = (Get-Content "nuget-key.txt" -Raw).Trim()
if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Error "nuget-key.txt is empty"
    exit 3
}

$gitIgnoreCheck = git check-ignore nuget-key.txt 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "nuget-key.txt is not gitignored"
    exit 3
}
Write-Success "nuget-key.txt is valid and gitignored"

# Step 4: Run tests (unless -SkipTests)
if (-not $SkipTests) {
    Write-Status "Running tests..."
    dotnet test --configuration Release --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit 4
    }
    Write-Success "All tests passed"
} else {
    Write-Status "Skipping tests (-SkipTests specified)"
}

# Step 5: Build and pack
Write-Status "Building and packing..."
dotnet pack LanSweeper.Api/LanSweeper.Api.csproj --configuration Release --include-symbols -p:SymbolPackageFormat=snupkg --output ./artifacts --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build/pack failed"
    exit 5
}

$packageFile = Get-ChildItem -Path ./artifacts -Filter "*.nupkg" |
    Where-Object { $_.Name -notmatch "\.snupkg$" } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $packageFile) {
    Write-Error "Package file not found"
    exit 5
}
Write-Success "Package created: $($packageFile.Name)"

# Step 6: Publish to NuGet
Write-Status "Publishing to NuGet.org..."
dotnet nuget push $packageFile.FullName --api-key $apiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit 6
}

Write-Success "Published $version to NuGet.org"
exit 0
