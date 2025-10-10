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

				// Don't retry on successful responses or client errors (4xx except 429)
				if (response.IsSuccessStatusCode ||
					(response.StatusCode >= HttpStatusCode.BadRequest &&
					 response.StatusCode < HttpStatusCode.InternalServerError &&
					 response.StatusCode != HttpStatusCode.TooManyRequests))
				{
					return response;
				}

				// For server errors or rate limiting, we may retry
				if (attempt < _options.MaxRetryAttempts)
				{
					_options.Logger?.LogWarning(
						"Request failed with status {StatusCode}. Attempt {Attempt} of {MaxAttempts}. Retrying...",
						response.StatusCode,
						attempt + 1,
						_options.MaxRetryAttempts);

					await DelayBeforeRetryAsync(attempt, cancellationToken).ConfigureAwait(false);
					attempt++;
					continue;
				}

				return response;
			}
			catch (HttpRequestException ex) when (attempt < _options.MaxRetryAttempts)
			{
				lastException = ex;

				_options.Logger?.LogWarning(
					ex,
					"Request failed with exception. Attempt {Attempt} of {MaxAttempts}. Retrying...",
					attempt + 1,
					_options.MaxRetryAttempts);

				await DelayBeforeRetryAsync(attempt, cancellationToken).ConfigureAwait(false);
				attempt++;
			}
			catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempt < _options.MaxRetryAttempts)
			{
				// Request timeout, not user cancellation
				lastException = ex;

				_options.Logger?.LogWarning(
					ex,
					"Request timed out. Attempt {Attempt} of {MaxAttempts}. Retrying...",
					attempt + 1,
					_options.MaxRetryAttempts);

				await DelayBeforeRetryAsync(attempt, cancellationToken).ConfigureAwait(false);
				attempt++;
			}
		}

		throw new LanSweeperException(
			$"Request failed after {_options.MaxRetryAttempts} retry attempts",
			lastException!);
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
