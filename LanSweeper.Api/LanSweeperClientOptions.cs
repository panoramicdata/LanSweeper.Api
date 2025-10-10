namespace LanSweeper.Api;

/// <summary>
/// Configuration options for the LanSweeper API client
/// </summary>
public sealed class LanSweeperClientOptions
{
	/// <summary>
	/// Required: Personal Access Token for authentication
	/// </summary>
	public required string AccessToken { get; init; }

	/// <summary>
	/// GraphQL API endpoint URL (default: https://api.lansweeper.com/api/v2/graphql)
	/// </summary>
	public string GraphQLEndpoint { get; init; } = "https://api.lansweeper.com/api/v2/graphql";

	/// <summary>
	/// Request timeout (default: 30 seconds)
	/// </summary>
	public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(30);

	/// <summary>
	/// Maximum retry attempts (default: 3)
	/// </summary>
	public int MaxRetryAttempts { get; init; } = 3;

	/// <summary>
	/// Retry delay (default: 1 second)
	/// </summary>
	public TimeSpan RetryDelay { get; init; } = TimeSpan.FromSeconds(1);

	/// <summary>
	/// Use exponential backoff for retries (default: true)
	/// </summary>
	public bool UseExponentialBackoff { get; init; } = true;

	/// <summary>
	/// Maximum retry delay (default: 30 seconds)
	/// </summary>
	public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromSeconds(30);

	/// <summary>
	/// Logger instance for diagnostic output
	/// </summary>
	public ILogger? Logger { get; init; }

	/// <summary>
	/// Enable request logging (default: false)
	/// </summary>
	public bool EnableRequestLogging { get; init; }

	/// <summary>
	/// Enable response logging (default: false)
	/// </summary>
	public bool EnableResponseLogging { get; init; }

	/// <summary>
	/// Validates the configuration options
	/// </summary>
	/// <exception cref="ArgumentException">Thrown when configuration is invalid</exception>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(AccessToken))
		{
			throw new ArgumentException("AccessToken is required", nameof(AccessToken));
		}

		if (string.IsNullOrWhiteSpace(GraphQLEndpoint))
		{
			throw new ArgumentException("GraphQLEndpoint is required", nameof(GraphQLEndpoint));
		}

		if (!Uri.TryCreate(GraphQLEndpoint, UriKind.Absolute, out _))
		{
			throw new ArgumentException("GraphQLEndpoint must be a valid URI", nameof(GraphQLEndpoint));
		}

		if (RequestTimeout <= TimeSpan.Zero)
		{
			throw new ArgumentException("RequestTimeout must be positive", nameof(RequestTimeout));
		}

		if (MaxRetryAttempts < 0)
		{
			throw new ArgumentException("MaxRetryAttempts cannot be negative", nameof(MaxRetryAttempts));
		}
	}
}
