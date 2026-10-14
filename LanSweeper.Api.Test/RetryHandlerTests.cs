namespace LanSweeper.Api.Test;

using System.Net;
using LanSweeper.Api.Infrastructure;

/// <summary>
/// Unit tests for RetryHandler
/// </summary>
public sealed class RetryHandlerTests
{
	private static LanSweeperClientOptions BuildOptions(
		int maxRetryAttempts,
		bool useExponentialBackoff,
		TimeSpan? maxRetryDelay,
		ILogger? logger)
	{
		return new LanSweeperClientOptions
		{
			AccessToken = "test-token",
			MaxRetryAttempts = maxRetryAttempts,
			RetryDelay = TimeSpan.FromMilliseconds(1),
			UseExponentialBackoff = useExponentialBackoff,
			MaxRetryDelay = maxRetryDelay ?? TimeSpan.Zero,
			Logger = logger
		};
	}

	private static HttpRequestMessage NewRequest()
		=> new(HttpMethod.Get, "https://api.example/graphql");

	private static async Task<HttpResponseMessage> SendWithRetryAsync(
		FakeInnerHandler inner,
		int maxRetryAttempts,
		bool useExponentialBackoff,
		TimeSpan? maxRetryDelay = null,
		ILogger? logger = null,
		CancellationToken? token = null)
	{
		using var handler = new RetryHandler(
			BuildOptions(maxRetryAttempts, useExponentialBackoff, maxRetryDelay, logger))
		{
			InnerHandler = inner
		};
		using var client = new HttpClient(handler);
		return await client.SendAsync(NewRequest(), token ?? CancellationToken.None);
	}

	[Fact]
	public async Task SendAsync_WhenSuccessImmediately_ReturnsSuccess()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK);

		// Act
		var response = await SendWithRetryAsync(inner, 2, useExponentialBackoff: true);

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

		// Act
		var response = await SendWithRetryAsync(inner, 3, useExponentialBackoff: true);

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

		// Act
		var response = await SendWithRetryAsync(inner, 2, useExponentialBackoff: true);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(2);
	}

	[Fact]
	public async Task SendAsync_WhenServerErrorEveryTime_RetriesUntilExhausted()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.InternalServerError);

		// Act
		var response = await SendWithRetryAsync(inner, 2, useExponentialBackoff: true);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		inner.Attempts.Should().Be(3);
	}

	[Fact]
	public async Task SendAsync_WhenBadRequest_DoesNotRetry()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.BadRequest);

		// Act
		var response = await SendWithRetryAsync(inner, 3, useExponentialBackoff: true);

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

		// Act
		var response = await SendWithRetryAsync(inner, 3, useExponentialBackoff: true);

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

		// Act
		var response = await SendWithRetryAsync(inner, 2, useExponentialBackoff: true);

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

		// Act
		var act = async () => await SendWithRetryAsync(inner, 3, useExponentialBackoff: true);

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

		// Act
		var act = async () => await SendWithRetryAsync(
			inner, 2, useExponentialBackoff: true, token: cts.Token);

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

		// Act
		var response = await SendWithRetryAsync(inner, 2, useExponentialBackoff: false);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task SendAsync_WhenDelayExceedsMax_UsesCap()
	{
		// Arrange
		var inner = new FakeInnerHandler(
			[HttpStatusCode.InternalServerError, HttpStatusCode.OK]);

		// Act
		var response = await SendWithRetryAsync(
			inner, 2, useExponentialBackoff: true, maxRetryDelay: TimeSpan.FromMilliseconds(1));

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

		// Act
		var response = await SendWithRetryAsync(inner, 3, useExponentialBackoff: true, logger: logger);

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
