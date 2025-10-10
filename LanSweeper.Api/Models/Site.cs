namespace LanSweeper.Api.Models;

/// <summary>
/// Represents a LanSweeper site
/// </summary>
public sealed class Site
{
	/// <summary>
	/// Gets or sets the site identifier
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; init; }

	/// <summary>
	/// Gets or sets the site name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets or sets the site description
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }
}
