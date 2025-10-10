namespace LanSweeper.Api.Models.Responses;

/// <summary>
/// Response for single asset query
/// </summary>
public sealed class AssetResponse
{
	/// <summary>
	/// Gets or sets the asset data
	/// </summary>
	[JsonPropertyName("asset")]
	public Asset? Asset { get; init; }
}
