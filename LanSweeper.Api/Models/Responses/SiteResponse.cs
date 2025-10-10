namespace LanSweeper.Api.Models.Responses;

/// <summary>
/// Response for site query
/// </summary>
public sealed class SiteResponse
{
	/// <summary>
	/// Gets or sets the site data
	/// </summary>
	[JsonPropertyName("site")]
	public Site? Site { get; init; }
}
