# LanSweeper.Api

[![Nuget](https://img.shields.io/nuget/v/PanoramicData.LanSweeper.Api)](https://www.nuget.org/packages/PanoramicData.LanSweeper.Api/)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/aaf15ba41ae648488d03d1128e24ff97)](https://app.codacy.com/gh/panoramicdata/LanSweeper.Api/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive .NET library for the LanSweeper Data API (GraphQL), providing easy access to LanSweeper's inventory and asset management data.

## Features

- **Full GraphQL Support**: Complete access to LanSweeper's GraphQL Data API
- **Type-Safe**: Strongly typed C# models for all API responses
- **Modern .NET**: Built for .NET 9 with modern C# patterns
- **Authentication**: Seamless Personal Access Token (PAT) authentication
- **Error Handling**: Comprehensive error handling and custom exceptions
- **Async/Await**: Full async support for non-blocking operations
- **Pagination**: Built-in support for GraphQL cursor-based pagination
- **Logging**: Integrated logging support via Microsoft.Extensions.Logging
- **Modular Design**: Organized API structure supporting multiple LanSweeper APIs

## Quick Start

### Installation

Install the package via NuGet:

```bash
dotnet add package LanSweeper.Api
```

### Basic Usage

```csharp
using LanSweeper.Api;

// Create client with Personal Access Token
var options = new LanSweeperClientOptions
{
    AccessToken = "your-personal-access-token"
};

var client = new LanSweeperClient(options);

// Get authorized sites
var sites = await client.Data.Sites.GetAllAsync();

// Get assets from a site
var assets = await client.Data.Assets.GetBySiteAsync("site-id");

// Execute custom GraphQL query
var result = await client.Data.Reports.ExecuteQueryAsync<CustomModel>("{ your custom query }");
```

## Authentication

LanSweeper.Api uses Personal Access Tokens (PATs) for authentication. To get started:

1. Log into your LanSweeper portal
2. Navigate to API Access settings
3. Generate a Personal Access Token
4. Use the token in your `LanSweeperClientOptions`

```csharp
var options = new LanSweeperClientOptions
{
    AccessToken = "your-personal-access-token",
    GraphQLEndpoint = "https://api.lansweeper.com/api/v2/graphql" // Optional: defaults to this
};
```

## API Structure

The client is organized into logical API groups:

### Data API (GraphQL)
Access to LanSweeper's GraphQL Data API for inventory and asset management:

```csharp
// Sites API
var sites = await client.Data.Sites.GetAllAsync();
var site = await client.Data.Sites.GetByIdAsync("site-id");

// Assets API
var assets = await client.Data.Assets.GetBySiteAsync("site-id");
var asset = await client.Data.Assets.GetByIdAsync("asset-key");

// Users API
var currentUser = await client.Data.Users.GetCurrentAsync();

// Reports API (Custom Queries)
var result = await client.Data.Reports.ExecuteQueryAsync<T>(query, variables);
```

## Available Modules

### Data API
- **Sites**: Manage and query LanSweeper scanning sites
- **Assets**: Access IT inventory and asset data
- **Reports**: Execute custom reports and queries
- **Users**: User and permissions management

*Note: Support for additional LanSweeper APIs (Device Recognition API, Platform API) coming in future releases.*

## Documentation

- [LanSweeper API Documentation](https://developer.lansweeper.com/docs/data-api/get-started/intro-to-graphql)
- [API Reference](https://developer.lansweeper.com/reference/schema)
- [Postman Collection](https://www.postman.com/lansweeper/lansweeper-s-public-workspace/overview)

## Requirements

- .NET 9.0 or later
- Valid LanSweeper account with API access
- Personal Access Token

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- [GitHub Issues](https://github.com/panoramicdata/LanSweeper.Api/issues)
- [LanSweeper Developer Portal](https://developer.lansweeper.com/)

---

**Note**: This is an unofficial library. LanSweeper is a trademark of Lansweeper NV.