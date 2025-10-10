# GitHub Copilot Instructions for LanSweeper.Api

## Project Overview
LanSweeper.Api is a comprehensive .NET library for the LanSweeper Data API (GraphQL), providing easy access to LanSweeper's inventory and asset management data.

## Key Technologies
- **Target Framework**: .NET 9
- **API Protocol**: GraphQL (not REST)
- **GraphQL Client**: GraphQL.Client library
- **Authentication**: Personal Access Token (Bearer token)
- **Testing**: xUnit v3 with Microsoft Testing Platform
- **Versioning**: Nerdbank.GitVersioning
- **Serialization**: System.Text.Json

## Code Style Guidelines

### Modern C# Patterns
- Use **file-scoped namespaces** for all new files
- Use **primary constructors** where appropriate
- Use **collection expressions** `[]` instead of `new List<>()`
- Use **required** properties for mandatory configuration
- Use **init-only** properties for immutable data

### Example File Structure
```csharp
namespace LanSweeper.Api.Models.Assets;

/// <summary>
/// LanSweeper IT asset information
/// </summary>
public sealed class Asset
{
    [JsonPropertyName("assetBasicInfo")]
    public AssetBasicInfo? BasicInfo { get; init; }
    
    [JsonPropertyName("assetCustom")]
    public AssetCustom? Custom { get; init; }
}
```

### GraphQL Specific Guidelines
- All GraphQL queries should be defined as string constants in `GraphQL/GraphQLQueries.cs`
- Use fragments for reusable query parts
- All models should have `[JsonPropertyName]` attributes matching GraphQL field names
- Response models should follow the pattern: `GraphQLResponse<T>` with `Data` and `Errors` properties

### API Structure
The API is organized into modules following this pattern:
```csharp
client.Sites.GetAllAsync()           // Sites management
client.Assets.GetByTypeAsync()       // Asset queries
client.Reports.ExecuteQueryAsync()   // Custom reports
client.Users.GetCurrentAsync()       // User information
```

### Error Handling
- Use custom exception hierarchy based on `LanSweeperException`
- Handle both HTTP errors and GraphQL errors in responses
- Always include proper error details and status codes

### Authentication
- Use Personal Access Tokens (PAT) with Bearer authentication
- No OAuth2 flows - simpler token-based auth
- Support token rotation and refresh capabilities

## Key Differences from HaloPSA.Api
- **Protocol**: GraphQL instead of REST
- **Client Library**: GraphQL.Client instead of Refit
- **Authentication**: Bearer tokens instead of OAuth2 client credentials
- **Response Format**: Nested GraphQL responses instead of direct JSON
- **Pagination**: Cursor-based instead of offset-based

## Testing Guidelines
- Integration tests should use user secrets for credentials
- Test against LanSweeper sandbox environment
- Aim for >80% code coverage
- Use AwesomeAssertions for fluent assertions
- Tests should be async and use CancellationToken

## Documentation Requirements
- All public APIs must have XML documentation
- Include `<example>` tags with usage examples
- Document all exceptions with `<exception>` tags
- Keep README.md updated with new features

## Dependencies to Avoid
- Do not use Refit (this is REST-focused)
- Do not use Newtonsoft.Json (use System.Text.Json)
- Avoid old testing frameworks (use xUnit v3)

## Performance Considerations
- Minimize GraphQL query count through batching
- Use cursor-based pagination for large datasets
- Implement retry logic with exponential backoff
- Cache frequently accessed data (sites, user info)

When generating code, follow these patterns and ensure compatibility with the GraphQL.Client library and LanSweeper's GraphQL API structure.