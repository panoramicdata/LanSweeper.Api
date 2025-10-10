namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Main interface for the LanSweeper API client
/// </summary>
public interface ILanSweeperClient : IDisposable
{
	/// <summary>
	/// Gets the Data API for GraphQL operations (sites, assets, users, reports)
	/// </summary>
	IDataApi Data { get; }
}
