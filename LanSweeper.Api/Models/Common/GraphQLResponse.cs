namespace LanSweeper.Api.Models.Common;

/// <summary>
/// GraphQL response wrapper
/// </summary>
/// <typeparam name="T">The data type</typeparam>
public sealed class GraphQLResponse<T>
{
	/// <summary>
	/// Gets or sets the response data
	/// </summary>
	[JsonPropertyName("data")]
	public T? Data { get; init; }

	/// <summary>
	/// Gets or sets the GraphQL errors
	/// </summary>
	[JsonPropertyName("errors")]
	public IReadOnlyList<GraphQLErrorDetail>? Errors { get; init; }

	/// <summary>
	/// Gets whether the response contains errors
	/// </summary>
	[JsonIgnore]
	public bool HasErrors => Errors?.Count > 0;
}

/// <summary>
/// GraphQL error details from the response
/// </summary>
public sealed class GraphQLErrorDetail
{
	/// <summary>
	/// Gets or sets the error message
	/// </summary>
	[JsonPropertyName("message")]
	public required string Message { get; init; }

	/// <summary>
	/// Gets or sets the path in the query where the error occurred
	/// </summary>
	[JsonPropertyName("path")]
	public IReadOnlyList<string>? Path { get; init; }

	/// <summary>
	/// Gets or sets additional error extensions
	/// </summary>
	[JsonPropertyName("extensions")]
	public Dictionary<string, object>? Extensions { get; init; }

	/// <summary>
	/// Gets or sets the locations in the query where the error occurred
	/// </summary>
	[JsonPropertyName("locations")]
	public IReadOnlyList<ErrorLocation>? Locations { get; init; }
}

/// <summary>
/// Error location in the GraphQL query
/// </summary>
public sealed class ErrorLocation
{
	/// <summary>
	/// Gets or sets the line number
	/// </summary>
	[JsonPropertyName("line")]
	public int Line { get; init; }

	/// <summary>
	/// Gets or sets the column number
	/// </summary>
	[JsonPropertyName("column")]
	public int Column { get; init; }
}
