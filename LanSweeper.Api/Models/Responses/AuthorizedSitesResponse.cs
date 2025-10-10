namespace LanSweeper.Api.Models.Responses;

/// <summary>
/// Response for authorized sites query
/// </summary>
public sealed class AuthorizedSitesResponse
{
	/// <summary>
	/// Gets or sets the authorized sites data
	/// </summary>
	[JsonPropertyName("authorizedSites")]
	public AuthorizedSitesData? AuthorizedSites { get; init; }
}

/// <summary>
/// Authorized sites data
/// </summary>
public sealed class AuthorizedSitesData
{
	/// <summary>
	/// Gets or sets the list of sites
	/// </summary>
	[JsonPropertyName("sites")]
	public IReadOnlyList<Site>? Sites { get; init; }
}
