namespace LanSweeper.Api.Api;

/// <summary>
/// API for managing LanSweeper sites
/// </summary>
/// <param name="client">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal sealed class SitesApi(GraphQLHttpClient client, ILogger? logger)
	: ApiBase(client, logger), ISitesApi
{
	/// <summary>
	/// Gets all authorized sites
	/// </summary>
	public async Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken)
	{
		Logger?.LogDebug("Getting all authorized sites");

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAuthorizedSites
		};

		var response = await SendQueryAsync<AuthorizedSitesResponse>(
			request,
			"Failed to retrieve authorized sites",
			cancellationToken)
			.ConfigureAwait(false);

		var sites = ExtractSites(response);

		Logger?.LogDebug("Retrieved {Count} authorized sites", sites.Count);

		return sites;
	}

	/// <summary>
	/// Gets a specific site by ID
	/// </summary>
	public async Task<Site> GetByIdAsync(string siteId, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(siteId);

		Logger?.LogDebug("Getting site by ID: {SiteId}", siteId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetSiteById,
			Variables = new
			{
				siteId
			}
		};

		var response = await SendQueryAsync<SiteResponse>(
			request,
			$"Failed to retrieve site with ID: {siteId}",
			cancellationToken)
			.ConfigureAwait(false);

		var site = response?.Site
			?? throw new LanSweeperNotFoundException($"Site with ID '{siteId}' not found");

		Logger?.LogDebug("Retrieved site: {SiteName}", site.Name);

		return site;
	}

	private static IReadOnlyList<Site> ExtractSites(AuthorizedSitesResponse? response)
		=> response?.AuthorizedSites?.Sites ?? [];
}
