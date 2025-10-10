namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Adds Token authentication to GraphQL requests
/// </summary>
internal sealed class AuthenticationHandler(LanSweeperClientOptions options) : DelegatingHandler
{
	private readonly LanSweeperClientOptions _options = options ?? throw new ArgumentNullException(nameof(options));

	/// <summary>
	/// Adds the Authorization header with Token for Personal Access Tokens
	/// </summary>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		// LanSweeper uses "Token" scheme for Personal Access Tokens, not "Bearer"
		request.Headers.Authorization = new AuthenticationHeaderValue("Token", _options.AccessToken);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
