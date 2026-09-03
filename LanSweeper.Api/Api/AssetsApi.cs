namespace LanSweeper.Api.Api;

/// <summary>
/// API for managing LanSweeper assets
/// </summary>
/// <param name="client">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal sealed class AssetsApi(GraphQLHttpClient client, ILogger? logger)
	: ApiBase(client, logger), IAssetsApi
{
	/// <summary>
	/// Gets assets from a specific site
	/// </summary>
	public async Task<IReadOnlyList<Asset>> GetBySiteAsync(
		string siteId,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(siteId);

		Logger?.LogDebug("Getting assets for site: {SiteId}", siteId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAssetsBySite,
			Variables = new
			{
				siteId,
				limit = 100
			}
		};

		var response = await SendQueryAsync<AssetsResponse>(
			request,
			$"Failed to retrieve assets for site: {siteId}",
			cancellationToken)
			.ConfigureAwait(false);

		var assets = ExtractAssets(response);

		Logger?.LogDebug(
			"Retrieved {Count} assets for site: {SiteId}",
			assets.Count,
			siteId);

		return assets;
	}

	/// <summary>
	/// Gets a specific asset by ID
	/// </summary>
	public async Task<Asset> GetByIdAsync(
		string assetId,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(assetId);

		Logger?.LogDebug("Getting asset by ID: {AssetId}", assetId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAssetById,
			Variables = new
			{
				assetId
			}
		};

		var response = await SendQueryAsync<AssetResponse>(
			request,
			$"Failed to retrieve asset with ID: {assetId}",
			cancellationToken)
			.ConfigureAwait(false);

		var asset = response?.Asset
			?? throw new LanSweeperNotFoundException($"Asset with ID '{assetId}' not found");

		Logger?.LogDebug("Retrieved asset: {AssetName}", asset.BasicInfo?.Name);

		return asset;
	}

	private static IReadOnlyList<Asset> ExtractAssets(AssetsResponse? response)
		=> response?.Site?.AssetResources?.Items ?? [];
}
