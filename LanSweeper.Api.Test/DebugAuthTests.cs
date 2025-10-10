namespace LanSweeper.Api.Test;

/// <summary>
/// Debug tests for authentication
/// </summary>
public sealed class DebugAuthTests
{
	[Fact]
	[Trait("Category", "Debug")]
	public void VerifyTokenFormat()
	{
		// Load the actual token from secrets
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<DebugAuthTests>()
			.Build();

		var token = configuration["LanSweeperApi:AccessToken"];

		// Verify token exists and has expected format
		_ = token.Should().NotBeNullOrEmpty("Token should be configured");
		_ = token.Should().StartWith("lsp_", "LanSweeper tokens should start with 'lsp_'");
		
		// Token should be reasonably long (typically 100+ characters)
		_ = token.Length.Should().BeGreaterThan(50, "Token should be a complete access token");

		Console.WriteLine($"Token length: {token.Length}");
		Console.WriteLine($"Token prefix: {token[..10]}...");
	}
}
