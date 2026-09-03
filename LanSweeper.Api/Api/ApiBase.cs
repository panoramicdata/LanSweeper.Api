namespace LanSweeper.Api.Api;

/// <summary>
/// Base class for the GraphQL-backed API surfaces, providing the shared client,
/// logger and error handling that every query goes through
/// </summary>
/// <param name="client">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal abstract class ApiBase(GraphQLHttpClient client, ILogger? logger)
{
	/// <summary>
	/// Gets the GraphQL HTTP client used to issue queries
	/// </summary>
	protected GraphQLHttpClient Client { get; } = client ?? throw new ArgumentNullException(nameof(client));

	/// <summary>
	/// Gets the optional logger instance
	/// </summary>
	protected ILogger? Logger { get; } = logger;

	/// <summary>
	/// Sends a GraphQL query and translates any GraphQL errors in the response into a
	/// <see cref="LanSweeperGraphQLException"/>
	/// </summary>
	/// <typeparam name="TResponse">The type the response data deserializes to</typeparam>
	/// <param name="request">The GraphQL request to send</param>
	/// <param name="failureMessage">The message to use if the response carries GraphQL errors</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>The response data, which may be null if the server returned none</returns>
	protected async Task<TResponse?> SendQueryAsync<TResponse>(
		GraphQLRequest request,
		string failureMessage,
		CancellationToken cancellationToken)
	{
		var response = await Client.SendQueryAsync<TResponse>(
			request,
			cancellationToken)
			.ConfigureAwait(false);

		if (response.Errors?.Length > 0)
		{
			var errors = response.Errors
				.Select(e => new Exceptions.GraphQLError { Message = e.Message })
				.ToList();

			throw new LanSweeperGraphQLException(failureMessage, errors);
		}

		return response.Data;
	}
}
