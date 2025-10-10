namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Exception thrown when rate limit is exceeded (HTTP 429)
/// </summary>
public sealed class LanSweeperRateLimitException : LanSweeperException
{
	/// <summary>
	/// Gets the time to wait before retrying, if provided by the API
	/// </summary>
	public TimeSpan? RetryAfter { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperRateLimitException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="errorDetails">Additional error details</param>
	/// <param name="retryAfter">The time to wait before retrying</param>
	public LanSweeperRateLimitException(string message, string? errorDetails = null, TimeSpan? retryAfter = null)
		: base(message, HttpStatusCode.TooManyRequests, errorDetails)
	{
		RetryAfter = retryAfter;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperRateLimitException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperRateLimitException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
