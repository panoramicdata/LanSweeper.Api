# LanSweeper.Api - API Coverage Documentation

## Overview

This document tracks the implementation status of LanSweeper GraphQL API endpoints in the LanSweeper.Api library.

**Last Updated**: January 2025  
**Library Version**: 1.0.0-preview  
**API Version**: LanSweeper Data API v2

---

## Coverage Summary

| Category | Implemented | Total | Coverage |
|----------|-------------|-------|----------|
| Sites API | 2 | 2 | 100% ? |
| Assets API | 2 | 4+ | 50% ?? |
| Users API | 1 | 2 | 50% ?? |
| Reports API | 1 | 1 | 100% ? |
| **Total** | **6** | **9+** | **67%** |

---

## Sites API

### Implemented ?

| Endpoint | Method | Status | Notes |
|----------|--------|--------|-------|
| Get All Sites | `Sites.GetAllAsync()` | ? Implemented | Returns all authorized sites |
| Get Site by ID | `Sites.GetByIdAsync(id)` | ? Implemented | Returns specific site details |

### Planned for Future Versions

| Endpoint | Target Version | Priority |
|----------|---------------|----------|
| Get Site Statistics | v1.1.0 | Medium |
| Get Site Configuration | v1.1.0 | Low |

---

## Assets API

### Implemented ?

| Endpoint | Method | Status | Notes |
|----------|--------|--------|-------|
| Get Assets by Site | `Assets.GetBySiteAsync(siteId)` | ? Implemented | Returns all assets for a site |
| Get Asset by ID | `Assets.GetByIdAsync(assetKey)` | ? Implemented | Returns specific asset details |

### Planned for Future Versions

| Endpoint | Target Version | Priority |
|----------|---------------|----------|
| Get Assets by Type | v1.1.0 | High |
| Search Assets | v1.1.0 | High |
| Get Asset Software | v1.1.0 | Medium |
| Get Asset Hardware | v1.1.0 | Medium |
| Get Asset History | v1.2.0 | Medium |
| Update Asset Custom Fields | v2.0.0 | Low |

---

## Users API

### Implemented ?

| Endpoint | Method | Status | Notes |
|----------|--------|--------|-------|
| Get Current User | `Users.GetCurrentAsync()` | ? Implemented | Returns authenticated user info |

### Planned for Future Versions

| Endpoint | Target Version | Priority |
|----------|---------------|----------|
| Get User Permissions | v1.1.0 | Medium |
| Get User Sites | v1.1.0 | Low |

---

## Reports API

### Implemented ?

| Endpoint | Method | Status | Notes |
|----------|--------|--------|-------|
| Execute Custom Query | `Reports.ExecuteQueryAsync<T>()` | ? Implemented | Execute any GraphQL query with variables |

### Notes:
- The Reports API provides full flexibility for custom GraphQL queries
- Users can execute any valid GraphQL query through this endpoint
- Support for variables and dynamic result types

---

## GraphQL Query Infrastructure

### Implemented Queries ?

| Query | Location | Status |
|-------|----------|--------|
| GetAuthorizedSites | `GraphQLQueries.cs` | ? |
| GetSiteById | `GraphQLQueries.cs` | ? |
| GetAssets | `GraphQLQueries.cs` | ? |
| GetAssetById | `GraphQLQueries.cs` | ? |
| GetCurrentUser | `GraphQLQueries.cs` | ? |

### Implemented Fragments ?

| Fragment | Purpose | Status |
|----------|---------|--------|
| AssetBasicFields | Basic asset information | ? |
| SiteFields | Basic site information | ? |

---

## Model Coverage

### Core Models ?

| Model | Properties | Status | Coverage |
|-------|-----------|--------|----------|
| `Site` | id, name | ? Complete | 100% |
| `Asset` | assetKey, assetBasicInfo, assetCustom | ? Complete | 80% |
| `AssetBasicInfo` | name, domain, ipAddress, mac, etc. | ? Complete | 100% |
| `AssetCustom` | manufacturer, model, serialNumber, etc. | ? Complete | 100% |
| `User` | userId, userName, email | ? Complete | 100% |

### Response Wrappers ?

| Wrapper | Purpose | Status |
|---------|---------|--------|
| `GraphQLResponse<T>` | Generic GraphQL response | ? Complete |
| `AuthorizedSitesResponse` | Sites query response | ? Complete |
| `SiteResponse` | Single site response | ? Complete |
| `AssetsResponse` | Assets query response | ? Complete |
| `AssetResponse` | Single asset response | ? Complete |
| `CurrentUserResponse` | User query response | ? Complete |

---

## Infrastructure Coverage

### Error Handling ?

| Exception Type | Purpose | Status |
|---------------|---------|--------|
| `LanSweeperException` | Base exception | ? |
| `LanSweeperAuthenticationException` | 401 errors | ? |
| `LanSweeperBadRequestException` | 400 errors | ? |
| `LanSweeperNotFoundException` | 404 errors | ? |
| `LanSweeperRateLimitException` | 429 errors | ? |
| `LanSweeperGraphQLException` | GraphQL errors | ? |

### HTTP Handlers ?

| Handler | Purpose | Status |
|---------|---------|--------|
| `AuthenticationHandler` | Bearer token auth | ? |
| `ErrorHandler` | Error processing | ? |
| `RetryHandler` | Retry logic | ? |
| `LoggingHandler` | Request/response logging | ? |

---

## Feature Coverage

### Core Features ?

| Feature | Status | Notes |
|---------|--------|-------|
| Bearer Token Authentication | ? | Personal Access Tokens |
| GraphQL Query Execution | ? | Full support |
| Custom Variable Support | ? | Dynamic variables |
| Response Deserialization | ? | System.Text.Json |
| Error Handling | ? | HTTP + GraphQL errors |
| Retry Logic | ? | Exponential backoff |
| Request Logging | ? | Microsoft.Extensions.Logging |
| Response Logging | ? | Microsoft.Extensions.Logging |
| Cancellation Token Support | ? | All async methods |
| Dispose Pattern | ? | Proper cleanup |

### Advanced Features

| Feature | Status | Target Version |
|---------|--------|---------------|
| Pagination (Manual) | ? v1.0.0 | Current |
| Pagination (IAsyncEnumerable) | ? v1.1.0 | Planned |
| Query Builder | ? v1.1.0 | Planned |
| Response Caching | ? v1.2.0 | Planned |
| Rate Limit Detection | ? v1.1.0 | Planned |
| Batch Queries | ? v2.0.0 | Future |
| GraphQL Subscriptions | ? v2.0.0 | Future |

---

## Testing Coverage

### Test Statistics

| Test Category | Tests | Passing | Coverage |
|--------------|-------|---------|----------|
| Unit Tests | 9 | 9 | 100% ? |
| Integration Tests | 21 | 17 | 81% ?? |
| Diagnostic Tests | 3 | 3 | 100% ? |
| **Total** | **33** | **29** | **88%** |

### Test Coverage by Module

| Module | Unit Tests | Integration Tests | Total Coverage |
|--------|-----------|------------------|---------------|
| Sites API | ? 100% | ?? 67% | 80% |
| Assets API | ? 100% | ?? 75% | 85% |
| Users API | ? 100% | ? 100% | 100% |
| Reports API | ? 100% | ?? 75% | 85% |
| Authentication | ? 100% | ?? 83% | 90% |
| Infrastructure | ? 100% | N/A | 100% |

---

## Known Limitations

### Current Version (v1.0.0-preview)

1. **Limited Asset Filtering**
   - Basic asset queries only
   - No type-specific filtering yet
   - Workaround: Use custom queries via Reports API

2. **No Pagination Helpers**
   - Manual pagination required
   - No IAsyncEnumerable support yet
   - Planned for v1.1.0

3. **Limited Asset Fields**
   - Basic and custom fields only
   - Hardware/software details not included
   - Full support planned for v1.1.0

4. **No Mutation Support**
   - Read-only operations only
   - Write operations planned for v2.0.0

### API Limitations

1. **Token Permissions**
   - Some queries may require elevated permissions
   - Documented in test failures

2. **Rate Limiting**
   - No automatic rate limit detection
   - Manual retry logic only
   - Planned for v1.1.0

---

## Roadmap

### v1.0.0 (Current) - Foundation Release
- ? Core API modules (Sites, Assets, Users, Reports)
- ? Basic authentication
- ? Error handling
- ? Custom query support
- ? Comprehensive documentation

### v1.1.0 (Next) - Enhancement Release
- ? Asset type filtering
- ? Advanced pagination
- ? Query builder helpers
- ? Rate limit detection
- ? Asset hardware/software details
- ? User permissions queries

### v1.2.0 (Future) - Performance Release
- ? Response caching
- ? Batch query optimization
- ? Connection pooling
- ? Performance metrics

### v2.0.0 (Future) - Advanced Release
- ? Mutation support (write operations)
- ? GraphQL subscriptions
- ? StrawberryShake integration (optional)
- ? Advanced query composition

---

## Contributing

To add support for new endpoints:

1. Add query definition to `GraphQL/GraphQLQueries.cs`
2. Create response model in `Models/Responses/`
3. Implement API method in appropriate API class
4. Add interface definition in `Interfaces/`
5. Write unit and integration tests
6. Update this coverage document

---

## References

- [LanSweeper API Documentation](https://developer.lansweeper.com/docs/data-api/get-started/intro-to-graphql)
- [GraphQL Schema Reference](https://developer.lansweeper.com/reference/schema)
- [Postman Collection](https://www.postman.com/lansweeper/lansweeper-s-public-workspace/overview)
- [GitHub Repository](https://github.com/panoramicdata/LanSweeper.Api)

---

**Document Version**: 1.0  
**Last Updated**: January 2025  
**Maintainer**: PanoramicData
