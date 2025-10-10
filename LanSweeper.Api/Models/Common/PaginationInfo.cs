namespace LanSweeper.Api.Models.Common;

/// <summary>
/// Cursor-based pagination information
/// </summary>
public sealed class PaginationInfo
{
	/// <summary>
	/// Gets or sets the number of items per page
	/// </summary>
	[JsonPropertyName("limit")]
	public int Limit { get; init; }

	/// <summary>
	/// Gets or sets the current cursor position
	/// </summary>
	[JsonPropertyName("current")]
	public string? Current { get; init; }

	/// <summary>
	/// Gets or sets the next cursor position
	/// </summary>
	[JsonPropertyName("next")]
	public string? Next { get; init; }

	/// <summary>
	/// Gets or sets the current page number
	/// </summary>
	[JsonPropertyName("page")]
	public int Page { get; init; }

	/// <summary>
	/// Gets whether there are more pages available
	/// </summary>
	[JsonIgnore]
	public bool HasNextPage => !string.IsNullOrEmpty(Next);
}
