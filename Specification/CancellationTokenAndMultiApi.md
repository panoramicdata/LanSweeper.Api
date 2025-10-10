# CancellationToken Required & Multi-API Support Update

**Date**: January 2025  
**Version**: 1.0.0  
**Type**: Breaking Change + Feature Addition

## Overview

This update implements two major changes:
1. **CancellationToken Made Required**: All async API methods now require explicit CancellationToken parameters
2. **Multi-API Support**: Extended implementation plan to cover all LanSweeper APIs (Data API, Device Recognition API, Platform API)

## Part 1: CancellationToken Required

### Rationale

Making CancellationToken parameters required is a best practice for async APIs:
- **Encourages Proper Cancellation**: Forces developers to think about cancellation scenarios
- **Prevents Forgotten Tokens**: No silent default behavior that might hide bugs
- **Aligns with Modern .NET Guidelines**: Microsoft recommends making CancellationToken required for public APIs
- **Test Quality**: Ensures all tests explicitly handle cancellation

### Changes Required

#### API Interfaces

**Before:**
```csharp
Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default);
```

**After:**
```csharp
Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken);
```

#### Files to Update

**Interfaces (5 files)**:
1. `LanSweeper.Api/Interfaces/ISitesApi.cs`
2. `LanSweeper.Api/Interfaces/IAssetsApi.cs`
3. `LanSweeper.Api/Interfaces/IUsersApi.cs`
4. `LanSweeper.Api/Interfaces/IReportsApi.cs`
5. `LanSweeper.Api/Interfaces/IDataApi.cs` (if it has methods)

**Implementations (4 files)**:
1. `LanSweeper.Api/Api/SitesApi.cs`
2. `LanSweeper.Api/Api/AssetsApi.cs`
3. `LanSweeper.Api/Api/UsersApi.cs`
4. `LanSweeper.Api/Api/ReportsApi.cs`

**Tests (0 files)**:
- Tests already use `CancellationToken` from `IntegrationTestBase`
- No test changes needed ?

### Migration Guide

Users will need to update their code:

**Before:**
```csharp
// Optional cancellation
var sites = await client.Data.Sites.GetAllAsync();
```

**After:**
```csharp
// Required cancellation (can use CancellationToken.None if no cancellation needed)
var sites = await client.Data.Sites.GetAllAsync(CancellationToken.None);

// Or with proper cancellation:
var cts = new CancellationTokenSource();
var sites = await client.Data.Sites.GetAllAsync(cts.Token);
```

## Part 2: Multi-API Support

### LanSweeper API Ecosystem

LanSweeper provides three distinct API collections (from Postman):

#### 1. Data API (GraphQL) ? IMPLEMENTED
**Endpoint**: `https://api.lansweeper.com/api/v2/graphql`  
**Protocol**: GraphQL  
**Purpose**: Inventory and asset management data

**Implemented:**
- Sites management
- Asset queries
- Custom reports
- User information

#### 2. Device Recognition API ? PLANNED (v1.2.0)
**Endpoint**: `https://api.lansweeper.com/api/devicerecognition/v2`  
**Protocol**: REST  
**Purpose**: Device identification and classification services

**Planned Features:**
- Device type identification by MAC address
- Manufacturer lookup
- Device model recognition
- Category classification

**Example Endpoints:**
```
POST /api/devicerecognition/v2/identify
GET /api/devicerecognition/v2/manufacturers
GET /api/devicerecognition/v2/categories
```

#### 3. Platform API ? PLANNED (v1.3.0)
**Endpoint**: `https://api.lansweeper.com/api/platform/v1`  
**Protocol**: REST  
**Purpose**: Platform management and administration

**Planned Features:**
- License management
- Integration configuration
- API key management
- Account settings

**Example Endpoints:**
```
GET /api/platform/v1/licenses
GET /api/platform/v1/integrations
POST /api/platform/v1/apikeys
```

### Updated Client Structure

```csharp
public interface ILanSweeperClient : IDisposable
{
    /// <summary>
    /// Data API (GraphQL) - Inventory and asset management
    /// </summary>
    IDataApi Data { get; }
    
    /// <summary>
    /// Device Recognition API (REST) - Device identification services
    /// v1.2.0+
    /// </summary>
    IDeviceRecognitionApi DeviceRecognition { get; }
    
    /// <summary>
    /// Platform API (REST) - Platform management and administration
    /// v1.3.0+
    /// </summary>
    IPlatformApi Platform { get; }
}
```

### Implementation Phases

#### Phase 8: Device Recognition API (v1.2.0)
**Duration**: 3-4 days  
**Dependencies**: v1.0.0 released

**Tasks**:
1. Create `IDeviceRecognitionApi` interface
2. Implement REST client for device recognition endpoints
3. Add device recognition models
4. Implement identification methods
5. Add manufacturer and category lookups
6. Write integration tests
7. Update documentation

**Key Components**:
```csharp
public interface IDeviceRecognitionApi
{
    /// <summary>
    /// Identify device by MAC address
    /// </summary>
    Task<DeviceIdentification> IdentifyByMacAsync(
        string macAddress, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Get all manufacturers
    /// </summary>
    Task<IReadOnlyList<Manufacturer>> GetManufacturersAsync(
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Get device categories
    /// </summary>
    Task<IReadOnlyList<DeviceCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken);
}
```

#### Phase 9: Platform API (v1.3.0)
**Duration**: 2-3 days  
**Dependencies**: v1.2.0 released

**Tasks**:
1. Create `IPlatformApi` interface
2. Implement REST client for platform endpoints
3. Add platform management models
4. Implement license queries
5. Implement integration management
6. Write integration tests
7. Update documentation

**Key Components**:
```csharp
public interface IPlatformApi
{
    /// <summary>
    /// Get license information
    /// </summary>
    Task<LicenseInfo> GetLicenseAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Get configured integrations
    /// </summary>
    Task<IReadOnlyList<Integration>> GetIntegrationsAsync(
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Get API keys
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetApiKeysAsync(
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Create new API key
    /// </summary>
    Task<ApiKey> CreateApiKeyAsync(
        ApiKeyRequest request, 
        CancellationToken cancellationToken);
}
```

### Technical Considerations

#### REST vs GraphQL

**Data API (GraphQL)**:
- Uses `GraphQL.Client` package
- Single endpoint with query-based operations
- Complex nested responses
- Custom fragment support

**Device Recognition & Platform APIs (REST)**:
- Can use `Refit` package or `HttpClient` directly
- Multiple endpoints
- Simple request/response patterns
- Standard HTTP verbs (GET, POST, PUT, DELETE)

#### Shared Infrastructure

All APIs will share:
- Authentication handler (Bearer token)
- Retry handler
- Logging handler
- Error handler (adapted for REST vs GraphQL)
- Client options
- Exception hierarchy

#### Client Options Update

```csharp
public sealed class LanSweeperClientOptions
{
    public required string AccessToken { get; init; }
    
    // API Endpoints
    public string DataApiEndpoint { get; init; } = 
        "https://api.lansweeper.com/api/v2/graphql";
    public string DeviceRecognitionApiEndpoint { get; init; } = 
        "https://api.lansweeper.com/api/devicerecognition/v2";
    public string PlatformApiEndpoint { get; init; } = 
        "https://api.lansweeper.com/api/platform/v1";
    
    // ...existing options...
}
```

### Roadmap Update

| Version | Focus | APIs | Duration | Status |
|---------|-------|------|----------|--------|
| v1.0.0 | Data API Foundation | Data API (GraphQL) | 3 days | ? Complete |
| v1.1.0 | Data API Enhancements | Data API improvements | 4 days | ? Planned |
| v1.2.0 | Device Recognition | + Device Recognition API | 4 days | ? Planned |
| v1.3.0 | Platform Management | + Platform API | 3 days | ? Planned |
| v2.0.0 | Advanced Features | All APIs + mutations | 5 days | ? Future |

### Benefits of Multi-API Support

1. **Complete Coverage**: Access all LanSweeper APIs from single library
2. **Consistent Interface**: Same authentication, error handling, logging across all APIs
3. **Unified Configuration**: Single client with shared settings
4. **Better Developer Experience**: IntelliSense shows all available operations
5. **Maintainability**: Shared infrastructure reduces code duplication

## Implementation Checklist

### Part 1: CancellationToken Required ?
- [ ] Update `ISitesApi.cs` - Remove all `= default`
- [ ] Update `IAssetsApi.cs` - Remove all `= default`
- [ ] Update `IUsersApi.cs` - Remove all `= default`
- [ ] Update `IReportsApi.cs` - Remove all `= default`
- [ ] Update `SitesApi.cs` - Remove all `= default`
- [ ] Update `AssetsApi.cs` - Remove all `= default`
- [ ] Update `UsersApi.cs` - Remove all `= default`
- [ ] Update `ReportsApi.cs` - Remove all `= default`
- [ ] Verify tests still pass (no changes needed)
- [ ] Update CHANGELOG.md - Document breaking change
- [ ] Update README.md - Update examples with required CancellationToken

### Part 2: Implementation Plan Update ?
- [ ] Update `Specification/ImplementationPlan.md`:
  - [ ] Add Phase 8: Device Recognition API
  - [ ] Add Phase 9: Platform API
  - [ ] Add API ecosystem overview
  - [ ] Add technical considerations
  - [ ] Update timeline and roadmap
  - [ ] Add API coverage targets

### Part 3: Documentation ?
- [ ] Update `Specification/ApiCoverage.md`:
  - [ ] Add Device Recognition API section (planned)
  - [ ] Add Platform API section (planned)
  - [ ] Update roadmap
- [ ] Create `Specification/MultiApiSupport.md` (this document)

## Testing Strategy

### CancellationToken Testing

**Existing Coverage**: ? All tests already use CancellationToken from base class

**Additional Tests to Add**:
```csharp
[Fact]
public async Task Operation_WithCancelledToken_ShouldThrowOperationCanceledException()
{
    // Arrange
    var cts = new CancellationTokenSource();
    cts.Cancel();
    
    // Act
    var act = async () => await Client.Data.Sites.GetAllAsync(cts.Token);
    
    // Assert
    await act.Should().ThrowAsync<OperationCanceledException>();
}
```

### Multi-API Testing (Future)

**v1.2.0 Tests**:
- Device identification accuracy tests
- Manufacturer lookup tests
- Category classification tests
- Error handling for invalid MAC addresses

**v1.3.0 Tests**:
- License info retrieval tests
- Integration listing tests
- API key CRUD operations tests
- Permission handling tests

## Impact Assessment

### CancellationToken Changes

**Impact**: LOW to MEDIUM
- **Existing Code**: Will need minor updates to pass CancellationToken
- **Migration Effort**: Simple - add `CancellationToken.None` or create token source
- **Benefits**: Better async patterns, improved cancellation support

### Multi-API Support

**Impact**: NONE (additive feature)
- **Existing Code**: No changes required
- **New Features**: Additional APIs available in future versions
- **Benefits**: Complete LanSweeper API coverage

## Timeline

### Immediate (This Update)
- [ ] Fix Cancellation Token signatures (1 hour)
- [ ] Update implementation plan (1 hour)
- [ ] Update documentation (30 minutes)
- [ ] Verify tests (15 minutes)

**Total**: 2.75 hours

### Future Releases
- **v1.1.0**: Data API enhancements (4 days)
- **v1.2.0**: Device Recognition API (4 days)
- **v1.3.0**: Platform API (3 days)

## References

- [LanSweeper Developer Portal](https://developer.lansweeper.com/)
- [Postman Workspace](https://www.postman.com/lansweeper/lansweeper-s-public-workspace/overview)
- [Data API Documentation](https://developer.lansweeper.com/docs/data-api/get-started/intro-to-graphql)
- [.NET CancellationToken Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)

---

**Document Version**: 1.0  
**Last Updated**: January 2025  
**Status**: Ready for Implementation  
**Affects**: v1.0.0 and later
