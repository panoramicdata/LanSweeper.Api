namespace LanSweeper.Api.Infrastructure;

/// <summary>Finds write operations in a GraphQL document without parsing it fully.</summary>
internal static class GraphQlOperationScanner
{
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
				i = ReadName(document, i, depth, out var isWrite);
				if (isWrite)
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

	private static int ReadName(string document, int start, int depth, out bool isWrite)
	{
		var end = start;
		while (end < document.Length && IsNameChar(document[end]))
		{
			end++;
		}

		isWrite = depth == 0 && IsWriteKeyword(document.AsSpan(start, end - start));
		return end;
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
