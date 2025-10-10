namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Interface for LanSweeper Data API operations (GraphQL)
/// </summary>
public interface IDataApi
{
	/// <summary>
	/// Gets the Sites API for managing LanSweeper sites
	/// </summary>
	ISitesApi Sites { get; }

	/// <summary>
	/// Gets the Assets API for querying and managing IT assets
	/// </summary>
	IAssetsApi Assets { get; }

	/// <summary>
	/// Gets the Reports API for custom queries and reports
	/// </summary>
	IReportsApi Reports { get; }

	/// <summary>
	/// Gets the Users API for user management
	/// </summary>
	IUsersApi Users { get; }
}
