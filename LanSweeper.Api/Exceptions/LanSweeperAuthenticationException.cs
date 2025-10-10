namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (HTTP 401)
/// </summary>
public sealed class LanSweeperAuthenticationException : LanSweeperException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperAuthenticationException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="errorDetails">Additional error details</param>
	public LanSweeperAuthenticationException(string message, string? errorDetails = null)
		: base(message, HttpStatusCode.Unauthorized, errorDetails)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperAuthenticationException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperAuthenticationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
