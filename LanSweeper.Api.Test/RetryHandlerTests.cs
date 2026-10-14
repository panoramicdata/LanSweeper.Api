namespace LanSweeper.Api.Test;

using System.Net;
using LanSweeper.Api.Infrastructure;

/// <summary>
/// Unit tests for RetryHandler
/// </summary>
public sealed class RetryHandlerTests
{
	private static LanSweeperClientOptions BuildOptions(
		int maxRetryAttempts = 2,
		bool useExponentialBackoff = true,
		TimeSpan maxRetryDelay = default,
		ILogger? logger = null)
	{
		if (maxRetryDelay == default)
		{
			return new LanSweeperClientOptions
			{
				AccessToken = "test-token",
				MaxRetryAttempts = maxRetryAttempts,
				RetryDelay = TimeSpan.FromMilliseconds(1),
				UseExponentialBackoff = useExponentialBackoff,
				Logger = logger
			};
		}

		return new LanSweeperClientOptions
		{
			AccessToken = "test-token",
			MaxRetryAttempts = maxRetryAttempts,
			RetryDelay = TimeSpan.FromMilliseconds(1),
			UseExponentialBackoff = useExponentialBackoff,
			MaxRetryDelay = maxRetryDelay,
			Logger = logger
		};
	}

	private static HttpRequestMessage NewRequest()
		=> new(HttpMethod.Get, "https://api.example/graphql");

	[Fact]
	public async Task SendAsync_WhenSuccessImmediately_ReturnsSuccess()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new RetryHandler(BuildOptions()) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(1);
	}

	[Fact]
	public async Task SendAsync_WhenServerErrorThenSuccess_RetriesAndSucceeds()
	{
		// Arrange
		var inner = new FakeInnerHandler(
			[HttpStatusCode.InternalServerError, HttpStatusCode.OK]);
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 3)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(2);
	}

	[Fact]
	public async Task SendAsync_WhenRateLimitedThenSuccess_Retries()
	{
		// Arrange
		var inner = new FakeInnerHandler(
			[HttpStatusCode.TooManyRequests, HttpStatusCode.OK]);
		using var handler = new RetryHandler(BuildOptions()) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(2);
	}

	[Fact]
	public async Task SendAsync_WhenServerErrorEveryTime_RetriesUntilExhausted()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.InternalServerError);
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 2)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		inner.Attempts.Should().Be(3);
	}

	[Fact]
	public async Task SendAsync_WhenBadRequest_DoesNotRetry()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.BadRequest);
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 3)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		inner.Attempts.Should().Be(1);
	}

	[Fact]
	public async Task SendAsync_WhenHttpRequestExceptionThenSuccess_Retries()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK)
		{
			FailFirstWith = new HttpRequestException("network down")
		};
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 3)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(2);
	}

	[Fact]
	public async Task SendAsync_WhenTaskCanceledException_Retries()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK)
		{
			FailFirstWith = new TaskCanceledException("timed out")
		};
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 2)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(2);
	}

	[Fact]
	public async Task SendAsync_WhenNonRetryableException_DoesNotRetry()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK)
		{
			FailFirstWith = new InvalidOperationException("not retryable")
		};
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 3)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var act = async () => await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		inner.Attempts.Should().Be(1);
	}

	[Fact]
	public async Task SendAsync_WithCallerCancellation_DoesNotRetryTaskCanceled()
	{
		// Arrange
		using var cts = new CancellationTokenSource();
		cts.Cancel();
		var inner = new FakeInnerHandler(HttpStatusCode.OK)
		{
			FailFirstWith = new TaskCanceledException("cancelled")
		};
		using var handler = new RetryHandler(BuildOptions(maxRetryAttempts: 2)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var act = async () => await client.SendAsync(NewRequest(), cts.Token);

		// Assert
		await act.Should().ThrowAsync<TaskCanceledException>();
		inner.Attempts.Should().Be(1);
	}

	[Fact]
	public async Task SendAsync_WithFixedDelay_Works()
	{
		// Arrange
		var inner = new FakeInnerHandler(
			[HttpStatusCode.InternalServerError, HttpStatusCode.OK]);
		using var handler = new RetryHandler(
			BuildOptions(maxRetryAttempts: 2, useExponentialBackoff: false)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task SendAsync_WhenDelayExceedsMax_UsesCap()
	{
		// Arrange
		var inner = new FakeInnerHandler(
			[HttpStatusCode.InternalServerError, HttpStatusCode.OK]);
		using var handler = new RetryHandler(
			BuildOptions(
				maxRetryAttempts: 2,
				useExponentialBackoff: true,
				maxRetryDelay: TimeSpan.FromMilliseconds(1))) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task SendAsync_WithLogger_LogsRetryWarning()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(
			[HttpStatusCode.InternalServerError, HttpStatusCode.OK]);
		using var handler = new RetryHandler(
			BuildOptions(maxRetryAttempts: 3, logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(NewRequest(), CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		logger.Messages.Should().Contain(m => m.Contains("Retrying"));
	}

	[Fact]
	public async Task SendAsync_WhenDelayCancelled_Throws()
	{
		// Arrange
		using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));
		var inner = new FakeInnerHandler(HttpStatusCode.InternalServerError);
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token",
			MaxRetryAttempts = 2,
			RetryDelay = TimeSpan.FromSeconds(10)
		};
		using var handler = new RetryHandler(options) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act & Assert - the Task.Delay observes the token and throws
		var act = async () => await client.SendAsync(NewRequest(), cts.Token);
		await act.Should().ThrowAsync<OperationCanceledException>();
	}
}

/// <summary>
/// Records formatted log messages emitted through <see cref="ILogger"/>.
/// </summary>
public sealed class CaptureLogger : ILogger
{
	private readonly List<string> _messages = [];

	public IReadOnlyList<string> Messages => _messages;

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		_messages.Add(formatter(state, exception));
	}
}

/// <summary>
/// A fake <see cref="HttpMessageHandler"/> that replays a fixed sequence of responses,
/// and optionally fails the first call with a configured exception.
/// </summary>
internal sealed class FakeInnerHandler : HttpMessageHandler
{
	private readonly List<HttpResponseMessage> _responses;

	public int Attempts { get; private set; }

	public Exception? FailFirstWith { get; init; }

	public FakeInnerHandler(HttpStatusCode status)
		: this(new[] { status })
	{
	}

	public FakeInnerHandler(IEnumerable<HttpStatusCode> statuses)
	{
		_responses = statuses
			.Select(s => new HttpResponseMessage(s) { Content = new StringContent("") })
			.ToList();
	}

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		Attempts++;

		if (Attempts == 1 && FailFirstWith is not null)
		{
			return Task.FromException<HttpResponseMessage>(FailFirstWith);
		}

		var index = Math.Min(Attempts - 1, _responses.Count - 1);
		return Task.FromResult(_responses[index]);
	}
}
