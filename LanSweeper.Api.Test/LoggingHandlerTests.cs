namespace LanSweeper.Api.Test;

using System.Net;
using LanSweeper.Api.Infrastructure;

/// <summary>
/// Unit tests for LoggingHandler
/// </summary>
public sealed class LoggingHandlerTests
{
	private static LanSweeperClientOptions BuildOptions(
		bool enableRequestLogging = true,
		bool enableResponseLogging = true,
		ILogger? logger = null)
		=> new()
		{
			AccessToken = "test-token",
			EnableRequestLogging = enableRequestLogging,
			EnableResponseLogging = enableResponseLogging,
			Logger = logger
		};

	[Fact]
	public async Task SendAsync_WithBothLoggingEnabled_LogsRequestAndResponse()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("{\"query\":\"{ hello }\"")
		};

		// Act
		var response = await client.SendAsync(request, CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithRequestContent_MasksBearerToken()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("Authorization: Bearer secret-token-123")
		};

		// Act
		await client.SendAsync(request, CancellationToken.None);

		// Assert
		logger.Messages.Should().Contain(m => m.Contains("***MASKED***"));
		logger.Messages.Should().NotContain(m => m.Contains("secret-token-123"));
	}

	[Fact]
	public async Task SendAsync_WithNonBearerContent_DoesNotMask()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("Authorization: Token my-token-456")
		};

		// Act
		await client.SendAsync(request, CancellationToken.None);

		// Assert
		logger.Messages.Should().Contain(m => m.Contains("my-token-456"));
	}

	[Fact]
	public async Task SendAsync_WithNoContent_DoesNotLogContent()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql");

		// Act
		await client.SendAsync(request, CancellationToken.None);

		// Assert
		logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		logger.Messages.Should().NotContain(m => m.Contains("Request Content"));
	}

	[Fact]
	public async Task SendAsync_WhenRequestLoggingDisabled_SkipsRequestLogging()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(enableRequestLogging: false, logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("{\"query\":\"{ hello }\"")
		};

		// Act
		await client.SendAsync(request, CancellationToken.None);

		// Assert
		logger.Messages.Should().NotContain(m => m.Contains("HTTP Request"));
		logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WhenResponseLoggingDisabled_SkipsResponseLogging()
	{
		// Arrange
		var logger = new CaptureLogger();
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(enableResponseLogging: false, logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql");

		// Act
		await client.SendAsync(request, CancellationToken.None);

		// Assert
		logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		logger.Messages.Should().NotContain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithLongResponse_TriggersTruncation()
	{
		// Arrange
		var logger = new CaptureLogger();
		var longBody = new string('x', 1500);
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(logger: logger)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		await client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql"),
			CancellationToken.None);

		// Assert
		logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithBothLoggingDisabled_ReturnsResponse()
	{
		// Arrange
		var inner = new FakeInnerHandler(HttpStatusCode.OK);
		using var handler = new LoggingHandler(
			BuildOptions(enableRequestLogging: false, enableResponseLogging: false)) { InnerHandler = inner };
		using var client = new HttpClient(handler);

		// Act
		var response = await client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql"),
			CancellationToken.None);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		inner.Attempts.Should().Be(1);
	}
}
