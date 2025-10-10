namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Exception thrown when a resource is not found (HTTP 404)
/// </summary>
public sealed class LanSweeperNotFoundException : LanSweeperException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperNotFoundException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="errorDetails">Additional error details</param>
	public LanSweeperNotFoundException(string message, string? errorDetails = null)
		: base(message, HttpStatusCode.NotFound, errorDetails)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperNotFoundException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
