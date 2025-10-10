namespace LanSweeper.Api.Api;

/// <summary>
/// API for managing LanSweeper sites
/// </summary>
internal sealed class SitesApi(GraphQLHttpClient client, ILogger? logger) : ISitesApi
{
	private readonly GraphQLHttpClient _client = client ?? throw new ArgumentNullException(nameof(client));
	private readonly ILogger? _logger = logger;

	/// <summary>
	/// Gets all authorized sites
	/// </summary>
	public async Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		_logger?.LogDebug("Getting all authorized sites");

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetAuthorizedSites
		};

		var response = await _client.SendQueryAsync<AuthorizedSitesResponse>(
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
				"Failed to retrieve authorized sites",
				errors);
		}

		var sites = response.Data?.AuthorizedSites?.Sites ?? [];

		_logger?.LogDebug("Retrieved {Count} authorized sites", sites.Count);

		return sites;
	}

	/// <summary>
	/// Gets a specific site by ID
	/// </summary>
	public async Task<Site> GetByIdAsync(string siteId, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(siteId);

		_logger?.LogDebug("Getting site by ID: {SiteId}", siteId);

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetSiteById,
			Variables = new
			{
				siteId
			}
		};

		var response = await _client.SendQueryAsync<SiteResponse>(
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
				$"Failed to retrieve site with ID: {siteId}",
				errors);
		}

		var site = response.Data?.Site
			?? throw new LanSweeperNotFoundException($"Site with ID '{siteId}' not found");

		_logger?.LogDebug("Retrieved site: {SiteName}", site.Name);

		return site;
	}
}
