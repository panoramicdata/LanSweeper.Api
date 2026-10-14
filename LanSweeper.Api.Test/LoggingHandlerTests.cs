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

	private sealed class TestSetup : IDisposable
	{
		public CaptureLogger Logger { get; } = new();
		public FakeInnerHandler Inner { get; } = new(HttpStatusCode.OK);
		public LoggingHandler Handler { get; }
		public HttpClient Client { get; }

		public TestSetup(bool enableRequestLogging = true, bool enableResponseLogging = true)
		{
			Handler = new LoggingHandler(BuildOptions(enableRequestLogging, enableResponseLogging, Logger))
			{
				InnerHandler = Inner
			};
			Client = new HttpClient(Handler);
		}

		public void Dispose()
		{
			Client.Dispose();
			Handler.Dispose();
		}
	}

	[Fact]
	public async Task SendAsync_WithBothLoggingEnabled_LogsRequestAndResponse()
	{
		using var setup = new TestSetup();
		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("{\"query\":\"{ hello }\"")
		};

		var response = await setup.Client.SendAsync(request, CancellationToken.None);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithRequestContent_MasksBearerToken()
	{
		using var setup = new TestSetup();
		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("Authorization: Bearer secret-token-123")
		};

		await setup.Client.SendAsync(request, CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("***MASKED***"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("secret-token-123"));
	}

	[Fact]
	public async Task SendAsync_WithNonBearerContent_DoesNotMask()
	{
		using var setup = new TestSetup();
		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("Authorization: Token my-token-456")
		};

		await setup.Client.SendAsync(request, CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("my-token-456"));
	}

	[Fact]
	public async Task SendAsync_WithNoContent_DoesNotLogContent()
	{
		using var setup = new TestSetup();
		var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql");

		await setup.Client.SendAsync(request, CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("Request Content"));
	}

	[Fact]
	public async Task SendAsync_WhenRequestLoggingDisabled_SkipsRequestLogging()
	{
		using var setup = new TestSetup(enableRequestLogging: false);
		var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example/graphql")
		{
			Content = new StringContent("{\"query\":\"{ hello }\"")
		};

		await setup.Client.SendAsync(request, CancellationToken.None);

		setup.Logger.Messages.Should().NotContain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WhenResponseLoggingDisabled_SkipsResponseLogging()
	{
		using var setup = new TestSetup(enableResponseLogging: false);
		var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql");

		await setup.Client.SendAsync(request, CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithLongResponse_TriggersTruncation()
	{
		using var setup = new TestSetup();

		await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql"),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithBothLoggingDisabled_ReturnsResponse()
	{
		using var setup = new TestSetup(enableRequestLogging: false, enableResponseLogging: false);

		var response = await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, "https://api.example/graphql"),
			CancellationToken.None);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		setup.Inner.Attempts.Should().Be(1);
	}
}
