using System.Buffers;
using System.Globalization;
using System.Text;

namespace System;

internal static class DomainNameHelper
{
	private static readonly SearchValues<char> s_unsafeForNormalizedHostChars = SearchValues.Create("\\/?@#:[]".AsSpan());

	private static readonly SearchValues<char> s_validChars = SearchValues.Create("-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz.".AsSpan());

	private static readonly SearchValues<char> s_iriInvalidChars = SearchValues.Create("\0\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,/:;<=>?@[\\]^`{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f".AsSpan());

	private static readonly SearchValues<char> s_asciiLetterUpperOrColonChars = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZ:".AsSpan());

	private static readonly IdnMapping s_idnMapping = new IdnMapping();

	internal static string ParseCanonicalName(string str, int start, int end, ref bool loopback)
	{
		int num = str.AsSpan(start, end - start).LastIndexOfAny(s_asciiLetterUpperOrColonChars);
		if (num >= 0 && str[start + num] == ':')
		{
			end = start + num;
			num = str.AsSpan(start, num).IndexOfAnyInRange('A', 'Z');
		}
		ReadOnlySpan<char> span = str.AsSpan(start, end - start);
		if (num >= 0)
		{
			if (span.Equals("localhost".AsSpan(), StringComparison.OrdinalIgnoreCase) || span.Equals("loopback".AsSpan(), StringComparison.OrdinalIgnoreCase))
			{
				loopback = true;
				return "localhost";
			}
			return System.UriHelper.SpanToLowerInvariantString(span);
		}
		if ((span.SequenceEqual("localhost".AsSpan()) || span.SequenceEqual("loopback".AsSpan())) ? true : false)
		{
			loopback = true;
			return "localhost";
		}
		return str.Substring(start, end - start);
	}

	public static bool IsValid(ReadOnlySpan<char> hostname, bool iri, bool notImplicitFile, out int length)
	{
		int num = (iri ? hostname.IndexOfAny(s_iriInvalidChars) : hostname.IndexOfAnyExcept(s_validChars));
		if (num >= 0)
		{
			char c = hostname[num];
			bool flag = ((c == '/' || c == '\\') ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = notImplicitFile;
				if (flag3)
				{
					bool flag4 = ((c == '#' || c == ':' || c == '?') ? true : false);
					flag3 = flag4;
				}
				flag2 = flag3;
			}
			if (!flag2)
			{
				length = 0;
				return false;
			}
			hostname = hostname.Slice(0, num);
		}
		length = hostname.Length;
		if (length == 0)
		{
			return false;
		}
		do
		{
			char c2 = hostname[0];
			if ((!iri || c2 < '\u00a0') && !char.IsAsciiLetterOrDigit(c2))
			{
				return false;
			}
			int num2 = (iri ? hostname.IndexOfAny(".。．｡".AsSpan()) : hostname.IndexOf('.'));
			int num3 = ((num2 < 0) ? hostname.Length : num2);
			if (iri)
			{
				ReadOnlySpan<char> readOnlySpan = hostname.Slice(0, num3);
				if (!Ascii.IsValid(readOnlySpan))
				{
					num3 += 4;
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
					for (int i = 0; i < readOnlySpan2.Length; i++)
					{
						if (readOnlySpan2[i] > 'ÿ')
						{
							num3++;
						}
					}
				}
			}
			if (!System.IriHelper.IsInInclusiveRange((uint)num3, 1u, 63u))
			{
				return false;
			}
			if (num2 < 0)
			{
				return true;
			}
			hostname = hostname.Slice(num2 + 1);
		}
		while (!hostname.IsEmpty);
		return true;
	}

	public static string IdnEquivalent(string hostname)
	{
		if (Ascii.IsValid(hostname.AsSpan()))
		{
			return hostname.ToLowerInvariant();
		}
		string unicode = System.UriHelper.StripBidiControlCharacters(hostname.AsSpan(), hostname);
		try
		{
			string ascii = s_idnMapping.GetAscii(unicode);
			if (ascii.AsSpan().ContainsAny(s_unsafeForNormalizedHostChars))
			{
				throw new UriFormatException(System.SR.net_uri_BadUnicodeHostForIdn);
			}
			return ascii;
		}
		catch (ArgumentException)
		{
			throw new UriFormatException(System.SR.net_uri_BadUnicodeHostForIdn);
		}
	}

	public static bool TryGetUnicodeEquivalent(string hostname, ref System.Text.ValueStringBuilder dest)
	{
		int num;
		for (num = 0; num < hostname.Length; num++)
		{
			if (num != 0)
			{
				dest.Append('.');
			}
			ReadOnlySpan<char> readOnlySpan = hostname.AsSpan(num);
			int num2 = readOnlySpan.IndexOfAny(".。．｡".AsSpan());
			if (num2 >= 0)
			{
				readOnlySpan = readOnlySpan.Slice(0, num2);
			}
			if (!Ascii.IsValid(readOnlySpan))
			{
				try
				{
					string ascii = s_idnMapping.GetAscii(hostname, num, readOnlySpan.Length);
					dest.Append(s_idnMapping.GetUnicode(ascii));
				}
				catch (ArgumentException)
				{
					return false;
				}
			}
			else
			{
				bool flag = false;
				if (readOnlySpan.StartsWith("xn--".AsSpan(), StringComparison.Ordinal))
				{
					try
					{
						dest.Append(s_idnMapping.GetUnicode(hostname, num, readOnlySpan.Length));
						flag = true;
					}
					catch (ArgumentException)
					{
					}
				}
				if (!flag)
				{
					readOnlySpan.ToLowerInvariant(dest.AppendSpan(readOnlySpan.Length));
				}
			}
			num += readOnlySpan.Length;
		}
		return true;
	}
}
