namespace LanSweeper.Api.Api;

/// <summary>
/// API for executing custom reports and queries
/// </summary>
internal sealed class ReportsApi(GraphQLHttpClient client, ILogger? logger) : IReportsApi
{
	private readonly GraphQLHttpClient _client = client ?? throw new ArgumentNullException(nameof(client));
	private readonly ILogger? _logger = logger;

	/// <summary>
	/// Executes a custom GraphQL query
	/// </summary>
	public async Task<T> ExecuteQueryAsync<T>(
		string query,
		Dictionary<string, object>? variables = null,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(query);

		_logger?.LogDebug("Executing custom GraphQL query");

		var request = new GraphQLRequest
		{
			Query = query,
			Variables = variables
		};

		var response = await _client.SendQueryAsync<T>(
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
				"Custom GraphQL query failed",
				errors);
		}

		if (response.Data is null)
		{
			throw new LanSweeperException("Query returned no data");
		}

		_logger?.LogDebug("Custom query executed successfully");

		return response.Data;
	}
}
