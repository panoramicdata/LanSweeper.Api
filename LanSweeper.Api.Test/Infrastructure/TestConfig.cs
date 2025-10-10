namespace LanSweeper.Api.Test.Infrastructure;

/// <summary>
/// Configuration for integration tests
/// </summary>
public sealed class TestConfig
{
	/// <summary>
	/// Gets or sets the Personal Access Token for API authentication
	/// </summary>
	public required string AccessToken { get; init; }

	/// <summary>
	/// Gets or sets whether to enable verbose logging
	/// </summary>
	public bool EnableVerboseLogging { get; init; }
}
