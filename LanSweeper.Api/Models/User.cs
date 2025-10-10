namespace LanSweeper.Api.Models;

/// <summary>
/// Represents a LanSweeper user
/// </summary>
public sealed class User
{
	/// <summary>
	/// Gets or sets the user identifier
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; init; }

	/// <summary>
	/// Gets or sets the user email
	/// </summary>
	[JsonPropertyName("email")]
	public string? Email { get; init; }

	/// <summary>
	/// Gets or sets the user name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }
}
