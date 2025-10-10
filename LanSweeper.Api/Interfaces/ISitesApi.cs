namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Interface for LanSweeper Sites API operations
/// </summary>
public interface ISitesApi
{
	/// <summary>
	/// Gets all authorized sites
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Collection of authorized sites</returns>
	Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Gets a specific site by ID
	/// </summary>
	/// <param name="siteId">The site identifier</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The requested site</returns>
	/// <exception cref="LanSweeperNotFoundException">Thrown when the site is not found</exception>
	Task<Site> GetByIdAsync(string siteId, CancellationToken cancellationToken);
}
