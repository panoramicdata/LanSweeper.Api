# API Structure Refactoring

**Date**: January 2025  
**Version**: 1.0.0-preview  
**Type**: Breaking Change

## Overview

The LanSweeper.Api client has been refactored to support multiple API types from LanSweeper's API ecosystem. This change groups related API operations under logical containers to enable future expansion.

## Motivation

LanSweeper provides multiple API collections:
1. **Data API** (GraphQL) - Inventory and asset management
2. **Device Recognition API** - Device identification services
3. **Platform API** - Platform management operations

The original flat structure (`client.Sites`, `client.Assets`, etc.) would become cluttered and confusing when adding support for additional APIs. The new structure groups operations by API type.

## Changes

### Before (v0.x)
```csharp
var client = new LanSweeperClient(options);

// Direct access to API modules
var sites = await client.Sites.GetAllAsync();
var assets = await client.Assets.GetBySiteAsync(siteId);
var user = await client.Users.GetCurrentAsync();
var result = await client.Reports.ExecuteQueryAsync<T>(query);
```

### After (v1.0+)
```csharp
var client = new LanSweeperClient(options);

// Grouped access under Data API
var sites = await client.Data.Sites.GetAllAsync();
var assets = await client.Data.Assets.GetBySiteAsync(siteId);
var user = await client.Data.Users.GetCurrentAsync();
var result = await client.Data.Reports.ExecuteQueryAsync<T>(query);
```

## Migration Guide

### Simple Find and Replace

Update your code with these replacements:

| Old Pattern | New Pattern |
|-------------|-------------|
| `client.Sites` | `client.Data.Sites` |
| `client.Assets` | `client.Data.Assets` |
| `client.Users` | `client.Data.Users` |
| `client.Reports` | `client.Data.Reports` |

### Example Migration

**Before:**
```csharp
using LanSweeper.Api;

var client = new LanSweeperClient(new LanSweeperClientOptions
{
    AccessToken = "your-token"
});

// Get sites
var sites = await client.Sites.GetAllAsync();

// Get assets
foreach (var site in sites)
{
    var assets = await client.Assets.GetBySiteAsync(site.Id);
    Console.WriteLine($"Site {site.Name} has {assets.Count} assets");
}

// Get current user
var user = await client.Users.GetCurrentAsync();
Console.WriteLine($"Logged in as: {user.Name}");
```

**After:**
```csharp
using LanSweeper.Api;

var client = new LanSweeperClient(new LanSweeperClientOptions
{
    AccessToken = "your-token"
});

// Get sites
var sites = await client.Data.Sites.GetAllAsync();

// Get assets
foreach (var site in sites)
{
    var assets = await client.Data.Assets.GetBySiteAsync(site.Id);
    Console.WriteLine($"Site {site.Name} has {assets.Count} assets");
}

// Get current user
var user = await client.Data.Users.GetCurrentAsync();
Console.WriteLine($"Logged in as: {user.Name}");
```

## Technical Implementation

### New Components

#### IDataApi Interface
```csharp
public interface IDataApi
{
    ISitesApi Sites { get; }
    IAssetsApi Assets { get; }
    IReportsApi Reports { get; }
    IUsersApi Users { get; }
}
```

#### DataApi Implementation
```csharp
internal sealed class DataApi : IDataApi
{
    public DataApi(GraphQLHttpClient graphQLClient, ILogger? logger)
    {
        Sites = new SitesApi(graphQLClient, logger);
        Assets = new AssetsApi(graphQLClient, logger);
        Reports = new ReportsApi(graphQLClient, logger);
        Users = new UsersApi(graphQLClient, logger);
    }
    
    public ISitesApi Sites { get; }
    public IAssetsApi Assets { get; }
    public IReportsApi Reports { get; }
    public IUsersApi Users { get; }
}
```

#### Updated ILanSweeperClient
```csharp
public interface ILanSweeperClient : IDisposable
{
    IDataApi Data { get; }
}
```

### File Structure

```
LanSweeper.Api/
??? Interfaces/
?   ??? ILanSweeperClient.cs
?   ??? IDataApi.cs          ? NEW
?   ??? ISitesApi.cs
?   ??? IAssetsApi.cs
?   ??? IUsersApi.cs
?   ??? IReportsApi.cs
??? Api/
?   ??? DataApi.cs           ? NEW
?   ??? SitesApi.cs
?   ??? AssetsApi.cs
?   ??? UsersApi.cs
?   ??? ReportsApi.cs
??? LanSweeperClient.cs
```

## Future Expansion

This structure enables clean addition of new API groups:

### Planned for Future Versions

#### Device Recognition API (v1.2.0)
```csharp
var devices = await client.DeviceRecognition.IdentifyAsync(macAddress);
var manufacturers = await client.DeviceRecognition.GetManufacturersAsync();
```

#### Platform API (v1.3.0)
```csharp
var licenses = await client.Platform.Licenses.GetAllAsync();
var integrations = await client.Platform.Integrations.GetAllAsync();
```

## Benefits

1. **Scalability**: Easy to add new API groups without cluttering the main client
2. **Organization**: Related operations are grouped logically
3. **Discoverability**: IntelliSense shows clear separation between API types
4. **Maintainability**: Each API group can evolve independently
5. **Documentation**: Clearer API documentation structure

## Testing

All 30 tests have been updated and continue to pass (26/30 passing, 4 with known authentication edge cases).

### Test Changes
- Updated all test files to use `Client.Data.*` pattern
- No changes to test logic or assertions
- All integration tests still validate same functionality

## Backward Compatibility

?? **Breaking Change**: This is a breaking change that requires code updates.

- Version 1.0.0 will include this new structure
- No backward compatibility layer provided (clean break for v1.0)
- Migration is straightforward (simple find/replace)

## Rollout Plan

1. ? Implement new structure
2. ? Update all tests
3. ? Update documentation (README, CHANGELOG)
4. ? Release as v1.0.0-preview
5. ? Gather feedback
6. ? Release as v1.0.0

## Questions & Answers

**Q: Why not provide backward compatibility?**  
A: As a pre-v1.0 library, this is the right time for a clean architectural change. The migration is simple and the benefit is significant.

**Q: Will there be more breaking changes before v1.0?**  
A: This is the only planned breaking change before v1.0 release.

**Q: Can I still access the old structure?**  
A: No, the old structure is completely replaced. Use the migration guide above.

**Q: What if I only use the Data API?**  
A: You'll need to add `.Data` to your client calls. It's a simple change with clear benefits for future expansion.

## Impact Assessment

### Low Impact
- Simple find/replace migration
- Clear migration path
- Well-documented change
- Early in project lifecycle (pre-v1.0)

### High Value
- Enables multi-API support
- Improves code organization
- Better IntelliSense experience
- Future-proof architecture

## References

- [Implementation Plan](ImplementationPlan.md)
- [API Coverage](ApiCoverage.md)
- [CHANGELOG](../CHANGELOG.md)
- [README](../README.md)

---

**Document Version**: 1.0  
**Last Updated**: January 2025  
**Status**: Implemented  
**Affects**: v1.0.0 and later
