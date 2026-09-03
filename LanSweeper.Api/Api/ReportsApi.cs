namespace LanSweeper.Api.Api;

/// <summary>
/// API for executing custom reports and queries
/// </summary>
/// <param name="client">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal sealed class ReportsApi(GraphQLHttpClient client, ILogger? logger)
	: ApiBase(client, logger), IReportsApi
{
	/// <summary>
	/// Executes a custom GraphQL query
	/// </summary>
	public async Task<T> ExecuteQueryAsync<T>(
		string query,
		Dictionary<string, object>? variables,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(query);

		Logger?.LogDebug("Executing custom GraphQL query");

		var request = new GraphQLRequest
		{
			Query = query,
			Variables = variables
		};

		var response = await SendQueryAsync<T>(
			request,
			"Custom GraphQL query failed",
			cancellationToken)
			.ConfigureAwait(false);

		if (response is null)
		{
			throw new LanSweeperException("Query returned no data");
		}

		Logger?.LogDebug("Custom query executed successfully");

		return response;
	}
}
