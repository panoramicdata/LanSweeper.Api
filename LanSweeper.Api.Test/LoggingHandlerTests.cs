namespace LanSweeper.Api.Test;

using System.Net;
using LanSweeper.Api.Infrastructure;

/// <summary>
/// Unit tests for LoggingHandler
/// </summary>
public sealed class LoggingHandlerTests
{
	private const string Url = "https://api.example/graphql";

	private static HttpRequestMessage BuildPostRequest(string body)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, Url);
		request.Content = new StringContent(body);
		return request;
	}

	private sealed class TestSetup : IDisposable
	{
		public CaptureLogger Logger { get; } = new();
		public FakeInnerHandler Inner { get; } = new(HttpStatusCode.OK);
		public LoggingHandler Handler { get; }
		public HttpClient Client { get; }

		public TestSetup(bool enableRequestLogging = true, bool enableResponseLogging = true)
		{
			Handler = new LoggingHandler(
				new LanSweeperClientOptions
				{
					AccessToken = "test-token",
					EnableRequestLogging = enableRequestLogging,
					EnableResponseLogging = enableResponseLogging,
					Logger = Logger
				})
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

		var response = await setup.Client.SendAsync(
			BuildPostRequest("{\"query\":\"{ hello }\"}"),
			CancellationToken.None);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithRequestContent_MasksBearerToken()
	{
		using var setup = new TestSetup();

		await setup.Client.SendAsync(
			BuildPostRequest("Authorization: Bearer secret-token-123"),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("***MASKED***"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("secret-token-123"));
	}

	[Fact]
	public async Task SendAsync_WithNonBearerContent_DoesNotMask()
	{
		using var setup = new TestSetup();

		await setup.Client.SendAsync(
			BuildPostRequest("Authorization: Token my-token-456"),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("my-token-456"));
	}

	[Fact]
	public async Task SendAsync_WithNoContent_DoesNotLogContent()
	{
		using var setup = new TestSetup();

		await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, Url),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("Request Content"));
	}

	[Fact]
	public async Task SendAsync_WhenRequestLoggingDisabled_SkipsRequestLogging()
	{
		using var setup = new TestSetup(enableRequestLogging: false);

		await setup.Client.SendAsync(
			BuildPostRequest("{\"query\":\"{ hello }\"}"),
			CancellationToken.None);

		setup.Logger.Messages.Should().NotContain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WhenResponseLoggingDisabled_SkipsResponseLogging()
	{
		using var setup = new TestSetup(enableResponseLogging: false);

		await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, Url),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Request"));
		setup.Logger.Messages.Should().NotContain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithLongResponse_TriggersTruncation()
	{
		using var setup = new TestSetup();

		await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, Url),
			CancellationToken.None);

		setup.Logger.Messages.Should().Contain(m => m.Contains("HTTP Response"));
	}

	[Fact]
	public async Task SendAsync_WithBothLoggingDisabled_ReturnsResponse()
	{
		using var setup = new TestSetup(enableRequestLogging: false, enableResponseLogging: false);

		var response = await setup.Client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, Url),
			CancellationToken.None);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		setup.Inner.Attempts.Should().Be(1);
	}
}
