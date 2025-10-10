namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Interface for LanSweeper Assets API operations
/// </summary>
public interface IAssetsApi
{
	/// <summary>
	/// Gets assets from a specific site
	/// </summary>
	/// <param name="siteId">The site identifier</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Collection of assets</returns>
	Task<IReadOnlyList<Asset>> GetBySiteAsync(string siteId, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific asset by ID
	/// </summary>
	/// <param name="assetId">The asset identifier</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The requested asset</returns>
	/// <exception cref="LanSweeperNotFoundException">Thrown when the asset is not found</exception>
	Task<Asset> GetByIdAsync(string assetId, CancellationToken cancellationToken);
}
