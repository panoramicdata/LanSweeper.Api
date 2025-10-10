namespace LanSweeper.Api.Models.Responses;

/// <summary>
/// Response for current user query
/// </summary>
public sealed class CurrentUserResponse
{
	/// <summary>
	/// Gets or sets the current user data
	/// </summary>
	[JsonPropertyName("me")]
	public User? Me { get; init; }
}
