# Publishing LanSweeper.Api to NuGet

This guide explains how to publish a new version of the LanSweeper.Api package to NuGet.org.

## Prerequisites

1. **PowerShell 7.0+** - [Download](https://github.com/PowerShell/PowerShell/releases)
2. **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
3. **NuGet API Key** - Get from [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
4. **Git** - For version control checks

## Setup

### 1. Create NuGet API Key

1. Go to https://www.nuget.org/account/apikeys
2. Click "Create"
3. Set the following:
   - **Key Name**: `LanSweeper.Api Publish`
   - **Glob Pattern**: `LanSweeper.Api`
   - **Select Scopes**: Check "Push"
4. Copy the generated API key
5. Create a file named `nuget-key.txt` in the solution root
6. Paste your API key into this file
7. The file is already in `.gitignore` so it won't be committed

```powershell
# Create the key file
echo "your-api-key-here" > nuget-key.txt
```

### 2. Verify Setup

```powershell
# Test that everything is working
.\Publish.ps1 -DryRun
```

## Publishing Process

### Quick Publish (Recommended)

```powershell
.\Publish.ps1
```

This will:
1. ? Check that NuGet packages are up to date
2. ? Ensure git working directory is clean (porcelain)
3. ? Build the solution in Release mode
4. ? Run all tests (requires 100% pass rate)
5. ? Pack NuGet package with symbols
6. ? Publish to NuGet.org
7. ? Create and optionally push git tag

### Options

```powershell
# Dry run - test without actually publishing
.\Publish.ps1 -DryRun

# Skip tests (NOT recommended for production)
.\Publish.ps1 -SkipTests

# Force publish even with uncommitted changes (NOT recommended)
.\Publish.ps1 -Force

# Combine options
.\Publish.ps1 -DryRun -SkipTests
```

## Version Numbering

The version follows **LanSweeper's version** using Nerdbank.GitVersioning:

- Base version in `version.json`: `12.5`
- Git height is automatically appended: `12.5.X`
- Example versions: `12.5.0`, `12.5.1`, `12.5.2`

The version aligns with LanSweeper's API version (currently 12.5) so users know compatibility.

## What Happens During Publish

### 1. Pre-Flight Checks ??

- Verifies PowerShell 7+, .NET 9, and Git are installed
- Checks for `nuget-key.txt`
- Scans for outdated NuGet packages
- Ensures git working directory is clean

### 2. Build & Test ???

- Cleans the solution
- Restores dependencies
- Builds in Release mode with zero warnings policy
- Runs all 30 tests (must achieve 100% pass rate)

### 3. Package ??

- Creates NuGet package with symbols (`.snupkg`)
- Outputs to `./artifacts/` directory
- Shows package size and details

### 4. Publish ??

- Uploads to NuGet.org
- Includes symbols for debugging
- Prompts for confirmation before publish

### 5. Tag ???

- Creates git tag (e.g., `v12.5.0`)
- Optionally pushes to origin

## After Publishing

### 1. Verify Package

Visit: https://www.nuget.org/packages/LanSweeper.Api/

It may take 5-10 minutes to appear in search results and package explorer.

### 2. Test Installation

```powershell
# Create test project
dotnet new console -o TestInstall
cd TestInstall

# Install package
dotnet add package LanSweeper.Api

# Verify it builds
dotnet build
```

### 3. Create GitHub Release

1. Go to https://github.com/panoramicdata/LanSweeper.Api/releases/new
2. Select the tag (e.g., `v12.5.0`)
3. Generate release notes or write custom notes
4. Publish release

### 4. Update Documentation

- [ ] Update README.md if needed
- [ ] Update CHANGELOG.md for next version
- [ ] Update API coverage docs if new features

### 5. Announce ??

Consider announcing on:
- Twitter/X
- LinkedIn
- Company blog
- GitHub Discussions

## Troubleshooting

### Error: "nuget-key.txt not found"

Create the file in the solution root:
```powershell
echo "your-api-key-here" > nuget-key.txt
```

### Error: "Git working directory is not clean"

Either commit/stash your changes, or use `-Force` (not recommended):
```powershell
git status
git add .
git commit -m "Prepare for v12.5.X release"
```

### Error: "Tests failed"

Fix failing tests before publishing:
```powershell
dotnet test --configuration Release
```

### Error: "Package with id 'LanSweeper.Api' and version 'X.X.X' already exists"

This version has already been published. You need to:
1. Make a new commit (this increments git height)
2. Retry publish

### Error: "Outdated packages detected"

Update packages before publishing:
```powershell
# See outdated packages
dotnet list package --outdated

# Update specific package
dotnet add package PackageName

# Or update all
# (Review changes carefully!)
```

## Security Notes

?? **NEVER commit `nuget-key.txt`** - It's in `.gitignore` but double-check!

The API key allows:
- ? Publishing new versions
- ? Unlisting versions
- ? Does NOT allow deleting versions (good!)

If your key is compromised:
1. Immediately revoke it at https://www.nuget.org/account/apikeys
2. Generate a new key
3. Update `nuget-key.txt`

## Manual Publish (if script fails)

If the PowerShell script doesn't work, you can publish manually:

```powershell
# Build and pack
dotnet pack LanSweeper.Api/LanSweeper.Api.csproj `
    --configuration Release `
    --include-symbols `
    --include-source `
    -p:SymbolPackageFormat=snupkg `
    --output ./artifacts

# Publish
dotnet nuget push ./artifacts/LanSweeper.Api.*.nupkg `
    --api-key YOUR-API-KEY-HERE `
    --source https://api.nuget.org/v3/index.json

# Tag
git tag -a v12.5.X -m "Release version 12.5.X"
git push origin v12.5.X
```

## CI/CD Integration

The `Publish.ps1` script can be integrated into CI/CD pipelines:

```yaml
# GitHub Actions example
- name: Publish to NuGet
  run: |
    echo "${{ secrets.NUGET_API_KEY }}" > nuget-key.txt
    pwsh -File Publish.ps1
```

## Support

If you encounter issues:

1. Check [NuGet Status](https://status.nuget.org/)
2. Review [NuGet Publishing Docs](https://docs.microsoft.com/en-us/nuget/create-packages/publish-a-package)
3. Open an issue on GitHub
4. Contact package maintainer

---

**Happy Publishing! ??**
