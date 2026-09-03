namespace LanSweeper.Api.Api;

/// <summary>
/// API for user management
/// </summary>
/// <param name="client">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal sealed class UsersApi(GraphQLHttpClient client, ILogger? logger)
	: ApiBase(client, logger), IUsersApi
{
	/// <summary>
	/// Gets the current authenticated user's information
	/// </summary>
	public async Task<User> GetCurrentAsync(CancellationToken cancellationToken)
	{
		Logger?.LogDebug("Getting current user information");

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetCurrentUser
		};

		var response = await SendQueryAsync<CurrentUserResponse>(
			request,
			"Failed to retrieve current user information",
			cancellationToken)
			.ConfigureAwait(false);

		var user = response?.Me
			?? throw new LanSweeperException("Current user information not available");

		Logger?.LogDebug("Retrieved current user: {UserEmail}", user.Email);

		return user;
	}
}
