namespace LanSweeper.Api.Exceptions;

/// <summary>
/// Exception thrown when GraphQL errors are returned in the response
/// </summary>
public sealed class LanSweeperGraphQLException : LanSweeperException
{
	/// <summary>
	/// Gets the collection of GraphQL errors from the response
	/// </summary>
	public IReadOnlyList<GraphQLError> Errors { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperGraphQLException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="errors">The collection of GraphQL errors</param>
	public LanSweeperGraphQLException(string message, IReadOnlyList<GraphQLError> errors)
		: base(message)
	{
		Errors = errors;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperGraphQLException"/> class
	/// </summary>
	/// <param name="message">The error message</param>
	/// <param name="innerException">The inner exception</param>
	public LanSweeperGraphQLException(string message, Exception innerException)
		: base(message, innerException)
	{
		Errors = [];
	}
}

/// <summary>
/// Represents a GraphQL error from the API response
/// </summary>
public sealed class GraphQLError
{
	/// <summary>
	/// Gets or sets the error message
	/// </summary>
	public required string Message { get; init; }

	/// <summary>
	/// Gets or sets the path in the query where the error occurred
	/// </summary>
	public IReadOnlyList<string>? Path { get; init; }

	/// <summary>
	/// Gets or sets additional error extensions
	/// </summary>
	public Dictionary<string, object>? Extensions { get; init; }
}
