using System.Buffers;
using System.Text.Unicode;

namespace System.Text.Encodings.Web;

internal sealed class DefaultJavaScriptEncoder : JavaScriptEncoder
{
	private sealed class EscaperImplementation : ScalarEscaperBase
	{
		internal static readonly EscaperImplementation Singleton = new EscaperImplementation(allowMinimalEscaping: false);

		internal static readonly EscaperImplementation SingletonMinimallyEscaped = new EscaperImplementation(allowMinimalEscaping: true);

		private readonly AsciiByteMap _preescapedMap;

		private EscaperImplementation(bool allowMinimalEscaping)
		{
			_preescapedMap.InsertAsciiChar('\b', 98);
			_preescapedMap.InsertAsciiChar('\t', 116);
			_preescapedMap.InsertAsciiChar('\n', 110);
			_preescapedMap.InsertAsciiChar('\f', 102);
			_preescapedMap.InsertAsciiChar('\r', 114);
			_preescapedMap.InsertAsciiChar('\\', 92);
			if (allowMinimalEscaping)
			{
				_preescapedMap.InsertAsciiChar('"', 34);
			}
		}

		internal override int EncodeUtf8(Rune value, Span<byte> destination)
		{
			if (_preescapedMap.TryLookup(value, out var value2))
			{
				if ((uint)destination.Length > 1u)
				{
					destination[0] = 92;
					destination[1] = value2;
					return 2;
				}
				return -1;
			}
			return TryEncodeScalarAsHex(this, value, destination);
			static int TryEncodeScalarAsHex(object @this, Rune rune, Span<byte> buffer)
			{
				if (rune.IsBmp)
				{
					if ((uint)buffer.Length > 5u)
					{
						buffer[0] = 92;
						buffer[1] = 117;
						System.HexConverter.ToBytesBuffer((byte)rune.Value, buffer, 4);
						System.HexConverter.ToBytesBuffer((byte)((uint)rune.Value >> 8), buffer, 2);
						return 6;
					}
				}
				else
				{
					UnicodeHelpers.GetUtf16SurrogatePairFromAstralScalarValue((uint)rune.Value, out var highSurrogate, out var lowSurrogate);
					if ((uint)buffer.Length > 11u)
					{
						buffer[0] = 92;
						buffer[1] = 117;
						System.HexConverter.ToBytesBuffer((byte)highSurrogate, buffer, 4);
						System.HexConverter.ToBytesBuffer((byte)((uint)highSurrogate >> 8), buffer, 2);
						buffer[6] = 92;
						buffer[7] = 117;
						System.HexConverter.ToBytesBuffer((byte)lowSurrogate, buffer, 10);
						System.HexConverter.ToBytesBuffer((byte)((uint)lowSurrogate >> 8), buffer, 8);
						return 12;
					}
				}
				return -1;
			}
		}

		internal override int EncodeUtf16(Rune value, Span<char> destination)
		{
			if (_preescapedMap.TryLookup(value, out var value2))
			{
				if ((uint)destination.Length > 1u)
				{
					destination[0] = '\\';
					destination[1] = (char)value2;
					return 2;
				}
				return -1;
			}
			return TryEncodeScalarAsHex(this, value, destination);
			static int TryEncodeScalarAsHex(object @this, Rune rune, Span<char> buffer)
			{
				if (rune.IsBmp)
				{
					if ((uint)buffer.Length > 5u)
					{
						buffer[0] = '\\';
						buffer[1] = 'u';
						System.HexConverter.ToCharsBuffer((byte)rune.Value, buffer, 4);
						System.HexConverter.ToCharsBuffer((byte)((uint)rune.Value >> 8), buffer, 2);
						return 6;
					}
				}
				else
				{
					UnicodeHelpers.GetUtf16SurrogatePairFromAstralScalarValue((uint)rune.Value, out var highSurrogate, out var lowSurrogate);
					if ((uint)buffer.Length > 11u)
					{
						buffer[0] = '\\';
						buffer[1] = 'u';
						System.HexConverter.ToCharsBuffer((byte)highSurrogate, buffer, 4);
						System.HexConverter.ToCharsBuffer((byte)((uint)highSurrogate >> 8), buffer, 2);
						buffer[6] = '\\';
						buffer[7] = 'u';
						System.HexConverter.ToCharsBuffer((byte)lowSurrogate, buffer, 10);
						System.HexConverter.ToCharsBuffer((byte)((uint)lowSurrogate >> 8), buffer, 8);
						return 12;
					}
				}
				return -1;
			}
		}
	}

	internal static readonly DefaultJavaScriptEncoder BasicLatinSingleton = new DefaultJavaScriptEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

	internal static readonly DefaultJavaScriptEncoder UnsafeRelaxedEscapingSingleton = new DefaultJavaScriptEncoder(new TextEncoderSettings(UnicodeRanges.All), allowMinimalJsonEscaping: true);

	private readonly OptimizedInboxTextEncoder _innerEncoder;

	public override int MaxOutputCharactersPerInputCharacter => 6;

	internal DefaultJavaScriptEncoder(TextEncoderSettings settings)
		: this(settings, allowMinimalJsonEscaping: false)
	{
	}

	private DefaultJavaScriptEncoder(TextEncoderSettings settings, bool allowMinimalJsonEscaping)
	{
		if (settings == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.settings);
		}
		_innerEncoder = (allowMinimalJsonEscaping ? new OptimizedInboxTextEncoder(EscaperImplementation.SingletonMinimallyEscaped, in settings.GetAllowedCodePointsBitmap(), forbidHtmlSensitiveCharacters: false, new char[2] { '"', '\\' }) : new OptimizedInboxTextEncoder(EscaperImplementation.Singleton, in settings.GetAllowedCodePointsBitmap(), forbidHtmlSensitiveCharacters: true, new char[2] { '\\', '`' }));
	}

	private protected override OperationStatus EncodeCore(ReadOnlySpan<char> source, Span<char> destination, out int charsConsumed, out int charsWritten, bool isFinalBlock)
	{
		return _innerEncoder.Encode(source, destination, out charsConsumed, out charsWritten, isFinalBlock);
	}

	private protected override OperationStatus EncodeUtf8Core(ReadOnlySpan<byte> utf8Source, Span<byte> utf8Destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
	{
		return _innerEncoder.EncodeUtf8(utf8Source, utf8Destination, out bytesConsumed, out bytesWritten, isFinalBlock);
	}

	private protected override int FindFirstCharacterToEncode(ReadOnlySpan<char> text)
	{
		return _innerEncoder.GetIndexOfFirstCharToEncode(text);
	}

	public unsafe override int FindFirstCharacterToEncode(char* text, int textLength)
	{
		return _innerEncoder.FindFirstCharacterToEncode(text, textLength);
	}

	public override int FindFirstCharacterToEncodeUtf8(ReadOnlySpan<byte> utf8Text)
	{
		return _innerEncoder.GetIndexOfFirstByteToEncode(utf8Text);
	}

	public unsafe override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
	{
		return _innerEncoder.TryEncodeUnicodeScalar(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
	}

	public override bool WillEncode(int unicodeScalar)
	{
		return !_innerEncoder.IsScalarValueAllowed(new Rune(unicodeScalar));
	}
}
