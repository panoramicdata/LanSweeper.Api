namespace LanSweeper.Api.Api;

/// <summary>
/// API for user management
/// </summary>
internal sealed class UsersApi(GraphQLHttpClient client, ILogger? logger) : IUsersApi
{
	private readonly GraphQLHttpClient _client = client ?? throw new ArgumentNullException(nameof(client));
	private readonly ILogger? _logger = logger;

	/// <summary>
	/// Gets the current authenticated user's information
	/// </summary>
	public async Task<User> GetCurrentAsync(CancellationToken cancellationToken = default)
	{
		_logger?.LogDebug("Getting current user information");

		var request = new GraphQLRequest
		{
			Query = GraphQLQueries.GetCurrentUser
		};

		var response = await _client.SendQueryAsync<CurrentUserResponse>(
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
				"Failed to retrieve current user information",
				errors);
		}

		var user = response.Data?.Me
			?? throw new LanSweeperException("Current user information not available");

		_logger?.LogDebug("Retrieved current user: {UserEmail}", user.Email);

		return user;
	}
}
