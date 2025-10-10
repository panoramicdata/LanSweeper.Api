namespace LanSweeper.Api.Interfaces;

/// <summary>
/// Interface for LanSweeper Users API operations
/// </summary>
public interface IUsersApi
{
	/// <summary>
	/// Gets the current authenticated user's information
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The current user's information</returns>
	Task<User> GetCurrentAsync(CancellationToken cancellationToken);
}
