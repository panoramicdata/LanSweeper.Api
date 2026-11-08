using System.Text.Json;

namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Refuses any GraphQL request that is not a query while <see cref="LanSweeperClientOptions.IsReadOnly"/> is set.
/// Every request is a POST of a GraphQL document, so the document is inspected: anything that is not provably
/// a query (a mutation, a subscription, or a body that cannot be read) is refused. The option is read on every request.
/// </summary>
internal sealed partial class ReadOnlyHandler(LanSweeperClientOptions options) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		if (options.IsReadOnly && !await IsReadAsync(request, cancellationToken).ConfigureAwait(false))
		{
			throw new InvalidOperationException("The client is read-only (IsReadOnly is true): only GraphQL queries are allowed.");
		}

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}

	private static async Task<bool> IsReadAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head || request.Method == HttpMethod.Options)
		{
			return true;
		}

		if (request.Method != HttpMethod.Post || request.Content is null)
		{
			return false;
		}

		var body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		return BodyIsQueryOnly(body);
	}

	private static bool BodyIsQueryOnly(string body)
	{
		try
		{
			using var document = JsonDocument.Parse(body);
			var root = document.RootElement;
			return root.ValueKind switch
			{
				JsonValueKind.Object => ObjectIsQueryOnly(root),
				JsonValueKind.Array => root.EnumerateArray().All(item => item.ValueKind == JsonValueKind.Object && ObjectIsQueryOnly(item)),
				_ => false
			};
		}
		catch (JsonException)
		{
			return false;
		}
	}

	private static bool ObjectIsQueryOnly(JsonElement request)
		=> request.TryGetProperty("query", out var query)
			&& query.ValueKind == JsonValueKind.String
			&& !HasWriteOperation(query.GetString()!);
}
