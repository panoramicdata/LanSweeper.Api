using System.Text.Json;

namespace LanSweeper.Api.Infrastructure;

/// <summary>
/// Refuses any GraphQL request that is not a query while <see cref="LanSweeperClientOptions.IsReadOnly"/> is set.
/// Every request is a POST of a GraphQL document, so the document is inspected: anything that is not provably
/// a query (a mutation, a subscription, or a body that cannot be read) is refused. The option is read on every request.
/// </summary>
internal sealed class ReadOnlyHandler(LanSweeperClientOptions options) : DelegatingHandler
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

	/// <summary>
	/// True when a top-level operation of the document is a mutation or subscription.
	/// Strings and comments are skipped and only identifiers outside any braces, parentheses or brackets are inspected.
	/// </summary>
	internal static bool HasWriteOperation(string document)
	{
		var depth = 0;
		var i = 0;
		while (i < document.Length)
		{
			var c = document[i];
			if (c == '#')
			{
				i = SkipComment(document, i);
			}
			else if (c == '"')
			{
				i = SkipString(document, i);
			}
			else if (IsNameStart(c))
			{
				var start = i;
				while (i < document.Length && IsNameChar(document[i]))
				{
					i++;
				}

				if (depth == 0 && IsWriteKeyword(document.AsSpan(start, i - start)))
				{
					return true;
				}
			}
			else
			{
				depth += DepthChange(c);
				i++;
			}
		}

		return false;
	}

	private static bool IsWriteKeyword(ReadOnlySpan<char> name)
		=> name.Equals("mutation", StringComparison.Ordinal) || name.Equals("subscription", StringComparison.Ordinal);

	private static int DepthChange(char c)
		=> c switch
		{
			'{' or '(' or '[' => 1,
			'}' or ')' or ']' => -1,
			_ => 0
		};

	private static bool IsNameStart(char c) => c == '_' || char.IsAsciiLetter(c);

	private static bool IsNameChar(char c) => c == '_' || char.IsAsciiLetterOrDigit(c);

	private static int SkipComment(string document, int index)
	{
		while (index < document.Length && document[index] != '\n' && document[index] != '\r')
		{
			index++;
		}

		return index;
	}

	private static int SkipString(string document, int index)
	{
		if (string.CompareOrdinal(document, index, "\"\"\"", 0, 3) == 0)
		{
			var end = document.IndexOf("\"\"\"", index + 3, StringComparison.Ordinal);
			return end < 0 ? document.Length : end + 3;
		}

		index++;
		while (index < document.Length && document[index] != '"')
		{
			index += document[index] == '\\' ? 2 : 1;
		}

		return index + 1;
	}
}
