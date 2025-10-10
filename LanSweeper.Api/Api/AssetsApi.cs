namespace LanSweeper.Api.Api;

/// <summary>
/// API for managing LanSweeper assets
/// </summary>
internal sealed class AssetsApi(GraphQLHttpClient client, ILogger? logger) : IAssetsApi
{
	private readonly GraphQLHttpClient _client = client ?? throw new ArgumentNullException(nameof(client));
	private readonly ILogger? _logger = logger;

	/// <summary>
	/// Gets assets from a specific site
	/// </summary>
	public async Task<IReadOnlyList<Asset>> GetBySiteAsync(
		string siteId,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(siteId);

		_logger?.LogDebug("Getting assets for site: {SiteId}", siteId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAssetsBySite,
			Variables = new
			{
				siteId,
				limit = 100
			}
		};

		var response = await _client.SendQueryAsync<AssetsResponse>(
			request,
			cancellationToken)
			.ConfigureAwait(false);

		// Check for GraphQL errors
		if (response.Errors?.Length > 0)
		{
			var errors = response.Errors
				.Select(e => new Exceptions.GraphQLError { Message = e.Message })
				.ToList();

			throw new LanSweeperGraphQLException(
				$"Failed to retrieve assets for site: {siteId}",
				errors);
		}

		var assets = response.Data?.Site?.AssetResources?.Items ?? [];

		_logger?.LogDebug(
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

		_logger?.LogDebug("Getting asset by ID: {AssetId}", assetId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAssetById,
			Variables = new
			{
				assetId
			}
		};

		var response = await _client.SendQueryAsync<AssetResponse>(
			request,
			cancellationToken)
			.ConfigureAwait(false);

		// Check for GraphQL errors
		if (response.Errors?.Length > 0)
		{
			var errors = response.Errors
				.Select(e => new Exceptions.GraphQLError { Message = e.Message })
				.ToList();

			throw new LanSweeperGraphQLException(
				$"Failed to retrieve asset with ID: {assetId}",
				errors);
		}

		var asset = response.Data?.Asset
			?? throw new LanSweeperNotFoundException($"Asset with ID '{assetId}' not found");

		_logger?.LogDebug("Retrieved asset: {AssetName}", asset.BasicInfo?.Name);

		return asset;
	}
}
