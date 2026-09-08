namespace System.Net;

internal static class CookieComparer
{
	internal static bool Equals(Cookie left, Cookie right)
	{
		if (!string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (!EqualDomains(left.Domain.AsSpan(), right.Domain.AsSpan()))
		{
			return false;
		}
		return string.Equals(left.Path, right.Path, StringComparison.Ordinal);
	}

	internal static bool EqualDomains(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		return StripLeadingDot(left).Equals(StripLeadingDot(right), StringComparison.OrdinalIgnoreCase);
	}

	internal static ReadOnlySpan<char> StripLeadingDot(ReadOnlySpan<char> s)
	{
		if (!s.StartsWith('.'))
		{
			return s;
		}
		return s.Slice(1, s.Length - 1);
	}
}
