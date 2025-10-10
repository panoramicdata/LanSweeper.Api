namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Handles HTTP and GraphQL errors from API responses
/// </summary>
internal sealed class ErrorHandler(ILogger? logger) : DelegatingHandler
{
	private readonly ILogger? _logger = logger;

	/// <summary>
	/// Processes the HTTP response and throws appropriate exceptions for error status codes
	/// </summary>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			await HandleHttpErrorAsync(response, cancellationToken).ConfigureAwait(false);
		}

		return response;
	}

	private async Task HandleHttpErrorAsync(
		HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		_logger?.LogError(
			"HTTP error {StatusCode}: {Content}",
			response.StatusCode,
			content);

		throw response.StatusCode switch
		{
			HttpStatusCode.Unauthorized =>
				new LanSweeperAuthenticationException("Authentication failed. Please check your access token.", content),

			HttpStatusCode.BadRequest =>
				new LanSweeperBadRequestException("Bad request. Please check your query parameters.", content),

			HttpStatusCode.NotFound =>
				new LanSweeperNotFoundException("Resource not found.", content),

			HttpStatusCode.TooManyRequests =>
				CreateRateLimitException(response, content),

			_ =>
				new LanSweeperException(
					$"API error: {response.StatusCode}",
					response.StatusCode,
					content)
		};
	}

	private static LanSweeperRateLimitException CreateRateLimitException(
		HttpResponseMessage response,
		string content)
	{
		TimeSpan? retryAfter = null;

		if (response.Headers.RetryAfter?.Delta is TimeSpan delta)
		{
			retryAfter = delta;
		}

		return new LanSweeperRateLimitException(
			"Rate limit exceeded. Please wait before making more requests.",
			content,
			retryAfter);
	}
}
