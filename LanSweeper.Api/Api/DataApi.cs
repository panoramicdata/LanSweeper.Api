using LanSweeper.Api.Api;

namespace LanSweeper.Api;

/// <summary>
/// Implementation of the Data API (GraphQL) for LanSweeper
/// </summary>
internal sealed class DataApi : IDataApi
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DataApi"/> class
	/// </summary>
	/// <param name="graphQLClient">The GraphQL HTTP client</param>
	/// <param name="logger">Optional logger instance</param>
	public DataApi(GraphQLHttpClient graphQLClient, ILogger? logger)
	{
		Sites = new SitesApi(graphQLClient, logger);
		Assets = new AssetsApi(graphQLClient, logger);
		Reports = new ReportsApi(graphQLClient, logger);
		Users = new UsersApi(graphQLClient, logger);
	}

	/// <summary>
	/// Gets the Sites API for managing LanSweeper sites
	/// </summary>
	public ISitesApi Sites { get; }

	/// <summary>
	/// Gets the Assets API for querying and managing IT assets
	/// </summary>
	public IAssetsApi Assets { get; }

	/// <summary>
	/// Gets the Reports API for custom queries and reports
	/// </summary>
	public IReportsApi Reports { get; }

	/// <summary>
	/// Gets the Users API for user management
	/// </summary>
	public IUsersApi Users { get; }
}
