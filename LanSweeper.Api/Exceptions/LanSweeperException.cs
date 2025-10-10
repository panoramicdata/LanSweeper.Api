namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Base exception for all LanSweeper API errors
/// </summary>
public class LanSweeperException : Exception
{
	/// <summary>
	/// Gets the HTTP status code associated with this exception, if applicable
	/// </summary>
	public HttpStatusCode? StatusCode { get; }

	/// <summary>
	/// Gets additional error details from the API response
	/// </summary>
	public string? ErrorDetails { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	public LanSweeperException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperException(string message, Exception innerException)
		: base(message, innerException) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="statusCode">The HTTP status code</param>
	/// <param name="errorDetails">Additional error details</param>
	public LanSweeperException(string message, HttpStatusCode statusCode, string? errorDetails = null)
		: base(message)
	{
		StatusCode = statusCode;
		ErrorDetails = errorDetails;
	}
}
