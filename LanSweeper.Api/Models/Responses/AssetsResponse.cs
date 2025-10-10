namespace LanSweeper.Api.Models.Responses;

/// <summary>
/// Response for assets query
/// </summary>
public sealed class AssetsResponse
{
	/// <summary>
	/// Gets or sets the site data containing asset resources
	/// </summary>
	[JsonPropertyName("site")]
	public SiteAssetsData? Site { get; init; }
}

/// <summary>
/// Site data containing asset resources
/// </summary>
public sealed class SiteAssetsData
{
	/// <summary>
	/// Gets or sets the asset resources
	/// </summary>
	[JsonPropertyName("assetResources")]
	public AssetResourcesData? AssetResources { get; init; }
}

/// <summary>
/// Asset resources data with pagination
/// </summary>
public sealed class AssetResourcesData
{
	/// <summary>
	/// Gets or sets the total number of assets
	/// </summary>
	[JsonPropertyName("total")]
	public int Total { get; init; }

	/// <summary>
	/// Gets or sets the list of assets
	/// </summary>
	[JsonPropertyName("items")]
	public IReadOnlyList<Asset>? Items { get; init; }

	/// <summary>
	/// Gets or sets the pagination information
	/// </summary>
	[JsonPropertyName("pagination")]
	public PaginationInfo? Pagination { get; init; }
}
