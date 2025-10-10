namespace LanSweeper.Api.Api;

/// <summary>
/// Implementation of the Data API (GraphQL) for LanSweeper
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DataApi"/> class
/// </remarks>
/// <param name="graphQLClient">The GraphQL HTTP client</param>
/// <param name="logger">Optional logger instance</param>
internal sealed class DataApi(GraphQLHttpClient graphQLClient, ILogger? logger) : IDataApi
{

	/// <summary>
	/// Gets the Sites API for managing LanSweeper sites
	/// </summary>
	public ISitesApi Sites { get; } = new SitesApi(graphQLClient, logger);

	/// <summary>
	/// Gets the Assets API for querying and managing IT assets
	/// </summary>
	public IAssetsApi Assets { get; } = new AssetsApi(graphQLClient, logger);

	/// <summary>
	/// Gets the Reports API for custom queries and reports
	/// </summary>
	public IReportsApi Reports { get; } = new ReportsApi(graphQLClient, logger);

	/// <summary>
	/// Gets the Users API for user management
	/// </summary>
	public IUsersApi Users { get; } = new UsersApi(graphQLClient, logger);
}
