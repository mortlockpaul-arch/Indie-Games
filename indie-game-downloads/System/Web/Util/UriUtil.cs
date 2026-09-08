using System.Diagnostics.CodeAnalysis;

namespace System.Web.Util;

internal static class UriUtil
{
	internal static bool TrySplitUriForPathEncode(string input, out ReadOnlySpan<char> schemeAndAuthority, [NotNullWhen(true)] out string path, out ReadOnlySpan<char> queryAndFragment)
	{
		int num = input.AsSpan().IndexOfAny('?', '#');
		string text;
		if (num >= 0)
		{
			text = input.Substring(0, num);
			queryAndFragment = input.AsSpan(num);
		}
		else
		{
			text = input;
			queryAndFragment = ReadOnlySpan<char>.Empty;
		}
		if (Uri.TryCreate(text, UriKind.Absolute, out Uri result))
		{
			string authority = result.Authority;
			if (!string.IsNullOrEmpty(authority))
			{
				int num2 = text.IndexOf(authority, StringComparison.OrdinalIgnoreCase);
				if (num2 >= 0)
				{
					int num3 = num2 + authority.Length;
					schemeAndAuthority = input.AsSpan(0, num3);
					path = text.Substring(num3);
					return true;
				}
			}
		}
		schemeAndAuthority = ReadOnlySpan<char>.Empty;
		path = null;
		queryAndFragment = ReadOnlySpan<char>.Empty;
		return false;
	}
}
