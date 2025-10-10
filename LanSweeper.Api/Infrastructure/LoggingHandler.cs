using System.Text.RegularExpressions;

namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Logs HTTP requests and responses for diagnostic purposes
/// </summary>
internal sealed partial class LoggingHandler(LanSweeperClientOptions options) : DelegatingHandler
{
	private readonly LanSweeperClientOptions _options = options ?? throw new ArgumentNullException(nameof(options));

	/// <summary>
	/// Sends the request and logs request/response details
	/// </summary>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		if (_options.EnableRequestLogging)
		{
			await LogRequestAsync(request, cancellationToken).ConfigureAwait(false);
		}

		var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

		if (_options.EnableResponseLogging)
		{
			await LogResponseAsync(response, cancellationToken).ConfigureAwait(false);
		}

		return response;
	}

	private async Task LogRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		_options.Logger?.LogDebug(
			"HTTP Request: {Method} {Uri}",
			request.Method,
			request.RequestUri);

		if (request.Content is not null)
		{
			var content = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

			// Mask sensitive data in access tokens
			var maskedContent = MaskSensitiveData(content);

			_options.Logger?.LogDebug("Request Content: {Content}", maskedContent);
		}
	}

	private async Task LogResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		_options.Logger?.LogDebug(
			"HTTP Response: {StatusCode} {ReasonPhrase}",
			(int)response.StatusCode,
			response.ReasonPhrase);

		if (response.Content is not null)
		{
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

			// Truncate very long responses
			var truncatedContent = content.Length > 1000
				? content[..1000] + "... (truncated)"
				: content;

			_options.Logger?.LogDebug("Response Content: {Content}", truncatedContent);
		}
	}

	private static string MaskSensitiveData(string content) =>
		// Basic masking of tokens - can be enhanced as needed
		content.Contains("Bearer ", StringComparison.OrdinalIgnoreCase)
			? SecretMaskingRegex().Replace(content, "Bearer ***MASKED***")
			: content;
	[GeneratedRegex(@"Bearer\s+[^\s""]+", RegexOptions.IgnoreCase, "en-GB")]
	private static partial Regex SecretMaskingRegex();
}
