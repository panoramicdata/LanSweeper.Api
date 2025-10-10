# LanSweeper.Api Integration Tests

## Setup Instructions

### 1. Configure User Secrets

The integration tests require a valid LanSweeper Personal Access Token. To set this up:

```bash
# Navigate to the test project directory
cd LanSweeper.Api.Test

# Initialize user secrets
dotnet user-secrets init

# Set your access token
dotnet user-secrets set "LanSweeperApi:AccessToken" "your-personal-access-token"

# Optional: Set a specific test site ID
dotnet user-secrets set "LanSweeperApi:TestSiteId" "your-site-id"

# Optional: Enable verbose logging
dotnet user-secrets set "LanSweeperApi:EnableVerboseLogging" "true"
```

### 2. Get Your Personal Access Token

1. Log into your LanSweeper cloud instance
2. Navigate to **Settings** ? **API**
3. Click **Generate Personal Access Token**
4. Copy the token and use it in the user secrets configuration above

### 3. Run the Tests

```bash
# Run all tests (including integration tests)
dotnet test

# Run only unit tests (skip integration tests)
dotnet test --filter "Category!=Integration"

# Run only integration tests
dotnet test --filter "Category=Integration"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Test Categories

- **Unit Tests**: Tests that don't require API access (validation, configuration, etc.)
- **Integration Tests**: Tests that make real API calls (require valid credentials)

## Test Structure

```
LanSweeper.Api.Test/
??? Infrastructure/
?   ??? IntegrationTestBase.cs      # Base class for integration tests
?   ??? TestConfig.cs                # Configuration model
??? IntegrationTests/
?   ??? SitesApiTests.cs            # Sites API integration tests
?   ??? AssetsApiTests.cs           # Assets API integration tests
?   ??? UsersApiTests.cs            # Users API integration tests
?   ??? ReportsApiTests.cs          # Custom queries tests
?   ??? AuthenticationTests.cs      # Auth & validation tests
??? LanSweeperClientTests.cs        # Client initialization tests
??? LanSweeperClientOptionsTests.cs # Options configuration tests
??? secrets.example.json            # Example secrets configuration
```

## Notes

- Integration tests will be skipped if no valid credentials are configured
- Tests use a 5-minute timeout for API operations
- Verbose logging can be enabled for debugging
- User secrets are stored locally and not committed to source control

## Troubleshooting

**Problem**: Tests fail with "Test configuration not found"
**Solution**: Make sure you've configured user secrets as described above

**Problem**: Authentication errors
**Solution**: Verify your Personal Access Token is valid and has not expired

**Problem**: No sites or assets returned
**Solution**: Ensure your LanSweeper account has access to sites and contains scanned assets
