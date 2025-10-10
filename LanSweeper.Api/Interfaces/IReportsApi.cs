namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Interface for LanSweeper Reports API operations
/// </summary>
public interface IReportsApi
{
	/// <summary>
	/// Executes a custom GraphQL query
	/// </summary>
	/// <typeparam name="T">The expected response type</typeparam>
	/// <param name="query">The GraphQL query string</param>
	/// <param name="variables">Optional query variables</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The query result</returns>
	/// <exception cref="LanSweeperGraphQLException">Thrown when the query contains errors</exception>
	Task<T> ExecuteQueryAsync<T>(
		string query,
		Dictionary<string, object>? variables = null,
		CancellationToken cancellationToken = default);
}
