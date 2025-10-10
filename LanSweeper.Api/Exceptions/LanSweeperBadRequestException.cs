namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Exception thrown when a bad request is made (HTTP 400)
/// </summary>
public sealed class LanSweeperBadRequestException : LanSweeperException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperBadRequestException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="errorDetails">Additional error details</param>
	public LanSweeperBadRequestException(string message, string? errorDetails = null)
		: base(message, HttpStatusCode.BadRequest, errorDetails)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperBadRequestException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperBadRequestException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
