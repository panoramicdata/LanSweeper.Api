# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- **BREAKING**: Refactored API structure to group Data API operations under `client.Data`
  - Changed: `client.Sites` ? `client.Data.Sites`
  - Changed: `client.Assets` ? `client.Data.Assets`
  - Changed: `client.Users` ? `client.Data.Users`
  - Changed: `client.Reports` ? `client.Data.Reports`
  - Reason: Enables support for additional LanSweeper APIs (Device Recognition API, Platform API) in future releases

### Known Issues
- 4 integration tests failing due to authentication edge cases (under investigation)
- Token permissions may affect some specific ID queries

## [1.0.0-preview] - 2025-01-XX

### Added
- **Core Infrastructure**
  - `LanSweeperClient` - Main API client with GraphQL support
  - `LanSweeperClientOptions` - Configuration with Bearer token authentication
  - Complete exception hierarchy (6 custom exception types)
  - HTTP handler chain (Authentication, Error, Retry, Logging)
  
- **API Modules**
  - `DataApi` - Container for Data API (GraphQL) operations
  - `SitesApi` - Get all sites, get site by ID
  - `AssetsApi` - Get assets by site, get asset by ID
  - `UsersApi` - Get current user information
  - `ReportsApi` - Execute custom GraphQL queries with variables
  
- **GraphQL Infrastructure**
  - Query definitions for all core operations
  - Reusable GraphQL fragments
  - Generic GraphQL response wrapper
  - Support for custom variables and dynamic queries
  
- **Models**
  - Site entity with full property mapping
  - Asset entity with basic info and custom fields
  - User entity with authentication details
  - Response wrappers for all API operations
  - Pagination support models
  
- **Testing**
  - 33 comprehensive tests (88% passing)
  - Unit tests for all core components
  - Integration tests for all API modules
  - Diagnostic tests for troubleshooting
  - User secrets configuration for secure testing
  
- **Documentation**
  - Complete README with quick start guide
  - XML documentation for all public APIs
  - API coverage documentation
  - Implementation plan and roadmap
  - Test setup instructions

### Features
- ? Personal Access Token (PAT) authentication
- ? Full GraphQL query support
- ? Custom variable support
- ? Comprehensive error handling
- ? Automatic retry with exponential backoff
- ? Request/response logging
- ? CancellationToken support
- ? Async/await throughout
- ? IDisposable implementation
- ? Modern .NET 9 patterns
- ? Modular API structure for future extensibility

### Technical Details
- Target Framework: .NET 9
- GraphQL Client: GraphQL.Client 6.1.0
- Serialization: System.Text.Json
- Testing: xUnit v3 with Microsoft Testing Platform
- Versioning: Nerdbank.GitVersioning
- Code Style: File-scoped namespaces, primary constructors, collection expressions

## [0.1.0] - 2025-01-09

### Added
- Initial project setup
- Repository structure creation
- Basic configuration files (.gitignore, .editorconfig, LICENSE)
- Initial README.md with project overview
- Specification folder with implementation plan

---

## Planned for Future Releases

### [1.1.0] - Enhancements (Planned)
- Asset filtering by type
- Advanced pagination with IAsyncEnumerable
- Query builder helpers
- Rate limit detection
- Asset hardware and software details
- User permissions queries

### [1.2.0] - Performance (Planned)
- Response caching
- Batch query optimization
- Connection pooling
- Performance metrics

### [2.0.0] - Advanced Features (Future)
- Mutation support (write operations)
- GraphQL subscriptions
- StrawberryShake integration (optional)
- Advanced query composition

---

## Template for Future Releases

```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- New features

### Changed
- Changes in existing functionality

### Deprecated
- Soon-to-be removed features

### Removed
- Removed features

### Fixed
- Bug fixes

### Security
- Security improvements