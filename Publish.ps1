#!/usr/bin/env pwsh
#Requires -Version 7.0

<#
.SYNOPSIS
    Publishes LanSweeper.Api package to NuGet.org

.DESCRIPTION
    This script performs the following steps:
 1. Checks that NuGet packages are up to date
    2. Ensures git working directory is clean (porcelain)
    3. Builds the solution in Release mode
    4. Runs all tests and ensures 100% pass rate
    5. Packs the NuGet package with symbols
    6. Publishes to NuGet.org using API key from nuget-key.txt

.PARAMETER SkipTests
    Skip running tests (not recommended for production releases)

.PARAMETER DryRun
    Perform all steps except the actual publish to NuGet

.PARAMETER Force
    Skip git porcelain check (not recommended)

.EXAMPLE
    .\Publish.ps1
    Standard publish workflow

.EXAMPLE
    .\Publish.ps1 -DryRun
    Test the publish process without actually publishing

.NOTES
    Requires:
    - PowerShell 7.0+
    - .NET 9 SDK
    - nuget-key.txt file in solution root (git-ignored)
#>

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$SkipTests,

    [Parameter()]
    [switch]$DryRun,

    [Parameter()]
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "[>>] $Message" -ForegroundColor Cyan
}

function Write-Warning {
  param([string]$Message)
    Write-Host "[!!] $Message" -ForegroundColor Yellow
}

function Write-Failure {
    param([string]$Message)
    Write-Host "[XX] $Message" -ForegroundColor Red
}

function Write-Step {
    param([string]$Message)
  Write-Host ""
    Write-Host "================================================================" -ForegroundColor Blue
    Write-Host " $Message" -ForegroundColor Blue
    Write-Host "================================================================" -ForegroundColor Blue
}

function Test-CommandExists {
    param([string]$Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

function Get-ProjectVersion {
    $versionJson = Get-Content "version.json" -Raw | ConvertFrom-Json
    $gitHeight = (git rev-list --count HEAD)
    return "$($versionJson.version).$gitHeight"
}

# Banner
Write-Host ""
Write-Host "================================================================" -ForegroundColor Magenta
Write-Host " LanSweeper.Api NuGet Publish Script" -ForegroundColor Magenta
Write-Host "================================================================" -ForegroundColor Magenta
Write-Host ""

$version = Get-ProjectVersion
Write-Info "Version: $version"
if ($DryRun) { Write-Warning "DRY RUN MODE - No actual publish will occur" }
Write-Host ""

# Step 1: Check Prerequisites
Write-Step "Step 1: Checking Prerequisites"

# Check PowerShell version
if ($PSVersionTable.PSVersion.Major -lt 7) {
    Write-Failure "PowerShell 7.0+ is required. Current version: $($PSVersionTable.PSVersion)"
    exit 1
}
Write-Success "PowerShell version: $($PSVersionTable.PSVersion)"

# Check .NET SDK
if (-not (Test-CommandExists 'dotnet')) {
    Write-Failure ".NET SDK not found. Please install .NET 9 SDK."
    exit 1
}
$dotnetVersion = dotnet --version
Write-Success ".NET SDK version: $dotnetVersion"

# Check git
if (-not (Test-CommandExists 'git')) {
    Write-Failure "Git not found. Please install Git."
    exit 1
}
$gitVersion = git --version
Write-Success "$gitVersion"

# Check for nuget-key.txt
if (-not (Test-Path "nuget-key.txt")) {
    Write-Failure "nuget-key.txt not found in solution root"
    Write-Info "Create this file with your NuGet API key from: https://www.nuget.org/account/apikeys"
    exit 1
}
Write-Success "NuGet API key file found"

# Step 2: Check NuGet Package Updates
Write-Step "Step 2: Checking for NuGet Package Updates"

Write-Info "Checking for outdated packages..."
try {
    $outdatedOutput = dotnet list package --outdated 2>&1 | Out-String
    
# Check if there are any outdated packages mentioned in the output
    if ($outdatedOutput -match "Top-level Package\s+Requested\s+Resolved\s+Latest") {
        # Parse the table to find outdated packages
        $lines = $outdatedOutput -split "`n"
        $hasOutdated = $false
    
        foreach ($line in $lines) {
    # Look for package lines (have version numbers)
            if ($line -match '^\s*>\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)') {
              $packageName = $matches[1]
$requested = $matches[2]
           $resolved = $matches[3]
  $latest = $matches[4]

         if ($resolved -ne $latest) {
       $hasOutdated = $true
    Write-Warning "Outdated: $packageName $resolved -> $latest"
}
            }
        }
        
        if ($hasOutdated) {
   Write-Warning "Some packages are outdated. Consider updating before publishing."
        $response = Read-Host "Continue anyway? (y/N)"
 if ($response -ne 'y' -and $response -ne 'Y') {
     Write-Info "Publish cancelled by user"
                exit 0
     }
        } else {
        Write-Success "All packages are up to date"
        }
    } else {
 Write-Success "All packages are up to date"
    }
} catch {
    Write-Warning "Could not check for outdated packages: $_"
    Write-Info "Continuing anyway..."
}

# Step 3: Check Git Status (Porcelain)
Write-Step "Step 3: Checking Git Status"

$gitStatus = git status --porcelain
if ($gitStatus -and -not $Force) {
    Write-Failure "Git working directory is not clean (porcelain)"
    Write-Host ""
    Write-Host "Uncommitted changes:" -ForegroundColor Yellow
    git status --short
    Write-Host ""
    Write-Info "Commit or stash changes before publishing, or use -Force to override"
    exit 1
} elseif ($gitStatus -and $Force) {
    Write-Warning "Git working directory is not clean, but -Force was specified"
} else {
    Write-Success "Git working directory is clean (porcelain)"
}

$currentBranch = git branch --show-current
Write-Info "Current branch: $currentBranch"

# Step 4: Clean Solution
Write-Step "Step 4: Cleaning Solution"

Write-Info "Running dotnet clean..."
dotnet clean --configuration Release --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Failure "Clean failed"
    exit 1
}
Write-Success "Solution cleaned"

# Step 5: Restore Dependencies
Write-Step "Step 5: Restoring Dependencies"

Write-Info "Running dotnet restore..."
dotnet restore --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Failure "Restore failed"
    exit 1
}
Write-Success "Dependencies restored"

# Step 6: Build Solution
Write-Step "Step 6: Building Solution (Release)"

Write-Info "Running dotnet build..."
dotnet build --configuration Release --no-restore --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Failure "Build failed"
    exit 1
}
Write-Success "Build completed successfully (0 warnings)"

# Step 7: Run Tests
if (-not $SkipTests) {
    Write-Step "Step 7: Running Tests"

Write-Info "Running dotnet test..."
    $testOutput = dotnet test --configuration Release --no-build --verbosity normal 2>&1
    $testExitCode = $LASTEXITCODE

    # Parse test results
    $testSummaryLine = $testOutput | Where-Object { $_ -match "Test run summary:" } | Select-Object -First 1
    $totalLine = $testOutput | Where-Object { $_ -match "^\s+total:" } | Select-Object -First 1
    $failedLine = $testOutput | Where-Object { $_ -match "^\s+failed:" } | Select-Object -First 1
    $succeededLine = $testOutput | Where-Object { $_ -match "^\s+succeeded:" } | Select-Object -First 1

    if ($totalLine -match "total:\s+(\d+)") { $totalTests = [int]$matches[1] }
    if ($failedLine -match "failed:\s+(\d+)") { $failedTests = [int]$matches[1] }
    if ($succeededLine -match "succeeded:\s+(\d+)") { $succeededTests = [int]$matches[1] }

    Write-Host ""
    Write-Info "Total Tests: $totalTests"
    Write-Info "Succeeded: $succeededTests"
    Write-Info "Failed: $failedTests"

    if ($testExitCode -ne 0 -or $failedTests -gt 0) {
        Write-Failure "Tests failed! Cannot publish with failing tests."
        Write-Host ""
   Write-Host "Test output:" -ForegroundColor Yellow
        $testOutput | Write-Host
        exit 1
    }

    $passRate = [math]::Round(($succeededTests / $totalTests) * 100, 1)
    Write-Success "All tests passed! ($passRate% pass rate)"
} else {
    Write-Warning "Tests skipped (not recommended for production releases)"
}

# Step 8: Pack NuGet Package
Write-Step "Step 8: Packing NuGet Package"

Write-Info "Running dotnet pack with symbols..."
dotnet pack LanSweeper.Api/LanSweeper.Api.csproj `
    --configuration Release `
    --no-build `
    --include-symbols `
    --include-source `
    -p:SymbolPackageFormat=snupkg `
    --output ./artifacts `
    --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Failure "Pack failed"
    exit 1
}

# Find the created package
$packageFile = Get-ChildItem -Path ./artifacts -Filter "LanSweeper.Api.*.nupkg" | 
    Where-Object { $_.Name -notmatch "\.symbols\.nupkg$" } | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1

$symbolsFile = Get-ChildItem -Path ./artifacts -Filter "LanSweeper.Api.*.snupkg" | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1

if (-not $packageFile) {
    Write-Failure "Package file not found in ./artifacts/"
    exit 1
}

Write-Success "Package created: $($packageFile.Name)"
if ($symbolsFile) {
    Write-Success "Symbols package created: $($symbolsFile.Name)"
}

# Show package info
$packageSize = [math]::Round($packageFile.Length / 1KB, 2)
Write-Info "Package size: $packageSize KB"

# Step 9: Publish to NuGet
Write-Step "Step 9: Publishing to NuGet.org"

if ($DryRun) {
    Write-Warning "DRY RUN - Skipping actual publish"
    Write-Info "Would publish: $($packageFile.FullName)"
    Write-Success "Dry run completed successfully!"
} else {
    # Read API key
    $apiKey = Get-Content "nuget-key.txt" -Raw
 $apiKey = $apiKey.Trim()

    Write-Info "Publishing package to NuGet.org..."
 Write-Warning "This will make version $version publicly available!"
    
    $response = Read-Host "Continue with publish? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Info "Publish cancelled by user"
        exit 0
    }

    dotnet nuget push $packageFile.FullName `
--api-key $apiKey `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate

    if ($LASTEXITCODE -ne 0) {
        Write-Failure "Publish failed"
        exit 1
    }

    Write-Success "Package published successfully!"
    Write-Host ""
    Write-Info "Package URL: https://www.nuget.org/packages/LanSweeper.Api/$version"
Write-Info "It may take a few minutes to appear in search results"
}

# Step 10: Create Git Tag
if (-not $DryRun) {
 Write-Step "Step 10: Creating Git Tag"

    $tagName = "v$version"
    $existingTag = git tag -l $tagName

    if ($existingTag) {
        Write-Warning "Tag $tagName already exists"
    } else {
        Write-Info "Creating tag: $tagName"
        git tag -a $tagName -m "Release version $version"
   
        $pushTag = Read-Host "Push tag to origin? (y/N)"
        if ($pushTag -eq 'y' -or $pushTag -eq 'Y') {
    git push origin $tagName
            Write-Success "Tag pushed to origin"
        } else {
    Write-Info "Tag created locally only. Push later with: git push origin $tagName"
        }
    }
}

# Summary
Write-Host ""
Write-Host "================================================================" -ForegroundColor Green
Write-Host "                 PUBLISH COMPLETE!" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Green
Write-Host ""
Write-Success "LanSweeper.Api $version published successfully!"
Write-Host ""

if (-not $DryRun) {
    Write-Info "Next steps:"
    Write-Host "  1. Verify package on NuGet.org: https://www.nuget.org/packages/LanSweeper.Api/" -ForegroundColor Cyan
    Write-Host "  2. Create GitHub release with release notes" -ForegroundColor Cyan
    Write-Host "  3. Announce on social media / blog" -ForegroundColor Cyan
Write-Host "  4. Update documentation if needed" -ForegroundColor Cyan
    Write-Host ""
}

Write-Success "Script completed successfully!"
exit 0
