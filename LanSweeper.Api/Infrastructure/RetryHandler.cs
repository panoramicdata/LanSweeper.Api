namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Implements retry logic with exponential backoff for transient failures
/// </summary>
internal sealed class RetryHandler(LanSweeperClientOptions options) : DelegatingHandler
{
	private readonly LanSweeperClientOptions _options = options ?? throw new ArgumentNullException(nameof(options));

	/// <summary>
	/// Sends the request with retry logic
	/// </summary>
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var attempt = 0;
		Exception? lastException = null;

		while (attempt <= _options.MaxRetryAttempts)
		{
			try
			{
				var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

				if (!ShouldRetry(response) || attempt >= _options.MaxRetryAttempts)
				{
					return response;
				}

				LogRetry(response.StatusCode, attempt);
			}
			catch (Exception ex) when (IsRetryable(ex, attempt, cancellationToken))
			{
				lastException = ex;
				LogRetry(ex, attempt);
			}

			await DelayBeforeRetryAsync(attempt, cancellationToken).ConfigureAwait(false);
			attempt++;
		}

		throw new LanSweeperException(
			$"Request failed after {_options.MaxRetryAttempts} retry attempts",
			lastException!);
	}

	/// <summary>
	/// Determines whether a response is worth retrying. Successful responses and client errors
	/// (4xx other than 429) are final; server errors and rate limiting may be retried.
	/// </summary>
	private static bool ShouldRetry(HttpResponseMessage response)
		=> !response.IsSuccessStatusCode
			&& (response.StatusCode < HttpStatusCode.BadRequest
				|| response.StatusCode >= HttpStatusCode.InternalServerError
				|| response.StatusCode == HttpStatusCode.TooManyRequests);

	/// <summary>
	/// Determines whether an exception represents a transient failure that may be retried
	/// </summary>
	private bool IsRetryable(Exception exception, int attempt, CancellationToken cancellationToken)
	{
		if (attempt >= _options.MaxRetryAttempts)
		{
			return false;
		}

		return exception switch
		{
			HttpRequestException => true,
			// A request timeout, as opposed to the caller cancelling
			TaskCanceledException => !cancellationToken.IsCancellationRequested,
			_ => false
		};
	}

	private void LogRetry(HttpStatusCode statusCode, int attempt)
		=> _options.Logger?.LogWarning(
			"Request failed with status {StatusCode}. Attempt {Attempt} of {MaxAttempts}. Retrying...",
			statusCode,
			attempt + 1,
			_options.MaxRetryAttempts);

	private void LogRetry(Exception exception, int attempt)
	{
		// Kept as two literal templates rather than one with the reason as a parameter, so
		// structured logging sinks can still group these by message.
		if (exception is TaskCanceledException)
		{
			_options.Logger?.LogWarning(
				exception,
				"Request timed out. Attempt {Attempt} of {MaxAttempts}. Retrying...",
				attempt + 1,
				_options.MaxRetryAttempts);

			return;
		}

		_options.Logger?.LogWarning(
			exception,
			"Request failed with exception. Attempt {Attempt} of {MaxAttempts}. Retrying...",
			attempt + 1,
			_options.MaxRetryAttempts);
	}

	private async Task DelayBeforeRetryAsync(int attempt, CancellationToken cancellationToken)
	{
		var delay = CalculateDelay(attempt);

		_options.Logger?.LogDebug("Waiting {Delay}ms before retry", delay.TotalMilliseconds);

		await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
	}

	private TimeSpan CalculateDelay(int attempt)
	{
		if (!_options.UseExponentialBackoff)
		{
			return _options.RetryDelay;
		}

		// Exponential backoff: delay * 2^attempt
		var exponentialDelay = TimeSpan.FromMilliseconds(
			_options.RetryDelay.TotalMilliseconds * Math.Pow(2, attempt));

		// Cap at maximum retry delay
		return exponentialDelay > _options.MaxRetryDelay
			? _options.MaxRetryDelay
			: exponentialDelay;
	}
}
