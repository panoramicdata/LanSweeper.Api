using System.Net;
using System.Text;
using LanSweeper.Api.Infrastructure;

namespace LanSweeper.Api.Test;

/// <summary>
/// Credential-free: a refused request throws before anything is sent.
/// </summary>
public sealed class ReadOnlyOptionTests
{
	private const string Mutation = "mutation { site(id: 1) { deleteCustomField(key: 2) } }";
	private const string Query = "query { authorizedSites { sites { id name } } }";

	private sealed class StubHandler : HttpMessageHandler
	{
		public List<HttpRequestMessage> Requests { get; } = [];

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Requests.Add(request);
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}
	}

	private static (HttpClient Client, StubHandler Stub) CreateClient(bool isReadOnly)
	{
		var stub = new StubHandler();
		var options = new LanSweeperClientOptions { AccessToken = "fake-token", IsReadOnly = isReadOnly };
		return (new HttpClient(new ReadOnlyHandler(options) { InnerHandler = stub }), stub);
	}

	private static StringContent Body(string graphQl)
		=> new($"{{\"query\":\"{graphQl}\"}}", Encoding.UTF8, "application/json");

	[Fact]
	public void IsReadOnly_DefaultsToFalse()
		=> new LanSweeperClientOptions { AccessToken = "fake-token" }.IsReadOnly.Should().BeFalse();

	[Fact]
	public async Task Mutation_IsRefused_WhenIsReadOnly()
	{
		var (client, stub) = CreateClient(isReadOnly: true);
		using var _ = client;

		var act = () => client.PostAsync("https://example.com/graphql", Body(Mutation), TestContext.Current.CancellationToken);

		await act.Should().ThrowAsync<InvalidOperationException>();
		stub.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task Mutation_IsAllowed_WhenNotReadOnly()
	{
		var (client, stub) = CreateClient(isReadOnly: false);
		using var _ = client;

		using var response = await client.PostAsync("https://example.com/graphql", Body(Mutation), TestContext.Current.CancellationToken);

		stub.Requests.Should().ContainSingle();
	}

	[Fact]
	public async Task Query_IsAllowed_WhenIsReadOnly()
	{
		var (client, stub) = CreateClient(isReadOnly: true);
		using var _ = client;

		using var response = await client.PostAsync("https://example.com/graphql", Body(Query), TestContext.Current.CancellationToken);

		stub.Requests.Should().ContainSingle();
	}

	[Fact]
	public async Task UnreadableBody_IsRefused_WhenIsReadOnly()
	{
		var (client, stub) = CreateClient(isReadOnly: true);
		using var _ = client;

		var act = () => client.PostAsync("https://example.com/graphql", new StringContent("not json"), TestContext.Current.CancellationToken);

		await act.Should().ThrowAsync<InvalidOperationException>();
		stub.Requests.Should().BeEmpty();
	}

	[Theory]
	[InlineData("query { a }", false)]
	[InlineData("{ a }", false)]
	[InlineData("query Q($m: String) { a(x: \"mutation\") }", false)]
	[InlineData("# mutation\nquery { mutation }", false)]
	[InlineData("mutation { a }", true)]
	[InlineData("query Q { a } mutation M { b }", true)]
	[InlineData("subscription { a }", true)]
	[InlineData("  \n mutation M { a }", true)]
	public void HasWriteOperation_ClassifiesDocuments(string document, bool expected)
		=> GraphQlOperationScanner.HasWriteOperation(document).Should().Be(expected);

	[Fact]
	public async Task ReportsMutation_IsRefused_ThroughTheClient_WhenIsReadOnly()
	{
		using var client = new LanSweeperClient(new LanSweeperClientOptions { AccessToken = "fake-token", IsReadOnly = true, MaxRetryAttempts = 0 });

		var act = () => client.Data.Reports.ExecuteQueryAsync<object>("mutation { a }", null, TestContext.Current.CancellationToken);

		var thrown = await act.Should().ThrowAsync<Exception>();
		thrown.Which.ToString().Should().Contain("IsReadOnly");
	}
}
