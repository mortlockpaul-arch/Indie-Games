using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Text.Json;

internal abstract class JsonSeparatorNamingPolicy : JsonNamingPolicy
{
	private enum SeparatorState
	{
		NotStarted,
		UppercaseLetter,
		LowercaseLetterOrDigit,
		SpaceSeparator
	}

	private readonly bool _lowercase;

	private readonly char _separator;

	internal JsonSeparatorNamingPolicy(bool lowercase, char separator)
	{
		_lowercase = lowercase;
		_separator = separator;
	}

	public sealed override string ConvertName(string name)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		return ConvertNameCore(_separator, _lowercase, name.AsSpan());
	}

	private static string ConvertNameCore(char separator, bool lowercase, ReadOnlySpan<char> chars)
	{
		char[] rentedBuffer = null;
		int num = (int)(1.2 * (double)chars.Length);
		Span<char> span = ((num > 128) ? ((Span<char>)(rentedBuffer = ArrayPool<char>.Shared.Rent(num))) : stackalloc char[128]);
		Span<char> destination = span;
		SeparatorState separatorState = SeparatorState.NotStarted;
		int charsWritten = 0;
		for (int i = 0; i < chars.Length; i++)
		{
			char c = chars[i];
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
			switch (unicodeCategory)
			{
			case UnicodeCategory.UppercaseLetter:
				switch (separatorState)
				{
				case SeparatorState.LowercaseLetterOrDigit:
				case SeparatorState.SpaceSeparator:
					WriteChar(separator, ref destination);
					break;
				case SeparatorState.UppercaseLetter:
					if (i + 1 < chars.Length && char.IsLower(chars[i + 1]))
					{
						WriteChar(separator, ref destination);
					}
					break;
				}
				if (lowercase)
				{
					c = char.ToLowerInvariant(c);
				}
				WriteChar(c, ref destination);
				separatorState = SeparatorState.UppercaseLetter;
				break;
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.DecimalDigitNumber:
				if (separatorState == SeparatorState.SpaceSeparator)
				{
					WriteChar(separator, ref destination);
				}
				if (!lowercase && unicodeCategory == UnicodeCategory.LowercaseLetter)
				{
					c = char.ToUpperInvariant(c);
				}
				WriteChar(c, ref destination);
				separatorState = SeparatorState.LowercaseLetterOrDigit;
				break;
			case UnicodeCategory.SpaceSeparator:
				if (separatorState != SeparatorState.NotStarted)
				{
					separatorState = SeparatorState.SpaceSeparator;
				}
				break;
			default:
				WriteChar(c, ref destination);
				separatorState = SeparatorState.NotStarted;
				break;
			}
		}
		string result = destination.Slice(0, charsWritten).ToString();
		if (rentedBuffer != null)
		{
			destination.Slice(0, charsWritten).Clear();
			ArrayPool<char>.Shared.Return(rentedBuffer);
		}
		return result;
		void ExpandBuffer(ref Span<char> reference)
		{
			int minimumLength = checked(reference.Length * 2);
			char[] array = ArrayPool<char>.Shared.Rent(minimumLength);
			reference.CopyTo(array);
			if (rentedBuffer != null)
			{
				reference.Slice(0, charsWritten).Clear();
				ArrayPool<char>.Shared.Return(rentedBuffer);
			}
			rentedBuffer = array;
			reference = rentedBuffer;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void WriteChar(char value, ref Span<char> reference)
		{
			if (charsWritten == reference.Length)
			{
				ExpandBuffer(ref reference);
			}
			reference[charsWritten++] = value;
		}
	}
}
