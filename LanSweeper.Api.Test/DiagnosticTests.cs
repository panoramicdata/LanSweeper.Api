#pragma warning disable xUnit1051 // Diagnostic tests don't need cancellation tokens

namespace LanSweeper.Api.Test;

/// <summary>
/// Diagnostic tool to test authentication and API connectivity
/// </summary>
public sealed class DiagnosticTests
{
	[Fact]
	[Trait("Category", "Diagnostic")]
	public async Task DiagnoseAuthentication()
	{
		// Load configuration
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<DiagnosticTests>()
			.Build();

		var token = configuration["LanSweeperApi:AccessToken"] ?? string.Empty;

		Console.WriteLine("=== LanSweeper API Diagnostic Tool ===");
		Console.WriteLine();
		Console.WriteLine($"Token configured: {!string.IsNullOrEmpty(token)}");
		Console.WriteLine($"Token length: {token.Length}");
		if (token.Length > 10)
		{
			Console.WriteLine($"Token prefix: {token[..10]}...");
		}

		Console.WriteLine();

		// Create a simple HTTP client to test authentication
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

		var endpoint = "https://api.lansweeper.com/api/v2/graphql";
		Console.WriteLine($"Testing endpoint: {endpoint}");
		Console.WriteLine();

		// Test 1: Simple query
		var query = """
			{
				"query": "query { authorizedSites { sites { id name } } }"
			}
			""";

		var content = new StringContent(query, System.Text.Encoding.UTF8, "application/json");

		try
		{
			Console.WriteLine("Sending test query...");
			var response = await httpClient.PostAsync(endpoint, content);

			Console.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");
			Console.WriteLine();

			var responseBody = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Response Body:");
			Console.WriteLine(responseBody);
			Console.WriteLine();

			// Check headers
			Console.WriteLine("Response Headers:");
			foreach (var header in response.Headers)
			{
				Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			Console.WriteLine($"Stack: {ex.StackTrace}");
		}

		// This test is for diagnostics, so we don't assert - just output info
		_ = true.Should().BeTrue();
	}

	[Fact]
	[Trait("Category", "Diagnostic")]
	public async Task TestAlternativeAuthHeaders()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<DiagnosticTests>()
			.Build();

		var token = configuration["LanSweeperApi:AccessToken"] ?? string.Empty;
		var endpoint = "https://api.lansweeper.com/api/v2/graphql";

		Console.WriteLine("=== Testing Alternative Auth Header Formats ===");
		Console.WriteLine();

		var query = """
			{
				"query": "query { authorizedSites { sites { id name } } }"
			}
			""";

		// Test different header formats
		var formats = new[]
		{
			("Authorization", $"Bearer {token}"),
			("Authorization", token),
			("X-API-Key", token),
			("api-key", token),
		};

		foreach (var (headerName, headerValue) in formats)
		{
			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add(headerName, headerValue);

			var content = new StringContent(query, System.Text.Encoding.UTF8, "application/json");

			try
			{
				var displayValue = headerValue.Length > 20 ? headerValue[..20] + "..." : headerValue;
				Console.WriteLine($"Testing: {headerName}: {displayValue}");
				var response = await httpClient.PostAsync(endpoint, content);
				var responseBody = await response.Content.ReadAsStringAsync();

				Console.WriteLine($"  Status: {(int)response.StatusCode}");
				var displayBody = responseBody.Length > 100 ? responseBody[..100] + "..." : responseBody;
				Console.WriteLine($"  Body: {displayBody}");
				Console.WriteLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"  Error: {ex.Message}");
				Console.WriteLine();
			}
		}

		_ = true.Should().BeTrue();
	}
}

#pragma warning restore xUnit1051
