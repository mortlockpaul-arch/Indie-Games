using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace System.Text.Encodings.Web;

internal sealed class OptimizedInboxTextEncoder
{
	private struct AsciiPreescapedData
	{
		private unsafe fixed ulong Data[128];

		internal unsafe void PopulatePreescapedData(in AllowedBmpCodePointsBitmap allowedCodePointsBmp, ScalarEscaperBase innerEncoder)
		{
			this = default(AsciiPreescapedData);
			byte* intPtr = stackalloc byte[16];
			// IL initblk instruction
			Unsafe.InitBlock(intPtr, 0, 16);
			Span<char> span = new Span<char>(intPtr, 8);
			for (int i = 0; i < 128; i++)
			{
				Rune value = new Rune(i);
				ulong num;
				int num2;
				if (!Rune.IsControl(value) && allowedCodePointsBmp.IsCharAllowed((char)i))
				{
					num = (uint)i;
					num2 = 1;
				}
				else
				{
					num2 = innerEncoder.EncodeUtf16(value, span.Slice(0, 6));
					num = 0uL;
					span.Slice(num2).Clear();
					for (int num3 = num2 - 1; num3 >= 0; num3--)
					{
						uint num4 = span[num3];
						num = (num << 8) | num4;
					}
				}
				Data[i] = num | ((ulong)(uint)num2 << 56);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe readonly bool TryGetPreescapedData(uint codePoint, out ulong preescapedData)
		{
			if (codePoint <= 127)
			{
				preescapedData = Data[codePoint];
				return true;
			}
			preescapedData = 0uL;
			return false;
		}
	}

	private readonly AsciiPreescapedData _asciiPreescapedData;

	private readonly AllowedBmpCodePointsBitmap _allowedBmpCodePoints;

	private readonly ScalarEscaperBase _scalarEscaper;

	private readonly SearchValues<byte> _allowedAsciiBytes;

	private readonly SearchValues<char> _allowedAsciiChars;

	internal OptimizedInboxTextEncoder(ScalarEscaperBase scalarEscaper, in AllowedBmpCodePointsBitmap allowedCodePointsBmp, bool forbidHtmlSensitiveCharacters = true, ReadOnlySpan<char> extraCharactersToEscape = default(ReadOnlySpan<char>))
	{
		_scalarEscaper = scalarEscaper;
		_allowedBmpCodePoints = allowedCodePointsBmp;
		_allowedBmpCodePoints.ForbidUndefinedCharacters();
		if (forbidHtmlSensitiveCharacters)
		{
			_allowedBmpCodePoints.ForbidHtmlCharacters();
		}
		ReadOnlySpan<char> readOnlySpan = extraCharactersToEscape;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char value = readOnlySpan[i];
			_allowedBmpCodePoints.ForbidChar(value);
		}
		_asciiPreescapedData.PopulatePreescapedData(in _allowedBmpCodePoints, scalarEscaper);
		Span<byte> span = stackalloc byte[128];
		Span<char> span2 = stackalloc char[128];
		int num = 0;
		for (int j = 0; j < 128; j++)
		{
			if (_allowedBmpCodePoints.IsCharAllowed((char)j))
			{
				span[num] = (byte)j;
				span2[num] = (char)j;
				num++;
			}
		}
		_allowedAsciiBytes = SearchValues.Create(span.Slice(0, num));
		_allowedAsciiChars = SearchValues.Create(span2.Slice(0, num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("FindFirstCharacterToEncode has been deprecated. It should only be used by the TextEncoder adapter.")]
	public unsafe int FindFirstCharacterToEncode(char* text, int textLength)
	{
		return GetIndexOfFirstCharToEncode(new ReadOnlySpan<char>(text, textLength));
	}

	[Obsolete("TryEncodeUnicodeScalar has been deprecated. It should only be used by the TextEncoder adapter.")]
	public unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
	{
		Span<char> destination = new Span<char>(buffer, bufferLength);
		if (_allowedBmpCodePoints.IsCodePointAllowed((uint)unicodeScalar))
		{
			if (!destination.IsEmpty)
			{
				destination[0] = (char)unicodeScalar;
				numberOfCharactersWritten = 1;
				return true;
			}
		}
		else
		{
			int num = _scalarEscaper.EncodeUtf16(new Rune(unicodeScalar), destination);
			if (num >= 0)
			{
				numberOfCharactersWritten = num;
				return true;
			}
		}
		numberOfCharactersWritten = 0;
		return false;
	}

	public OperationStatus Encode(ReadOnlySpan<char> source, Span<char> destination, out int charsConsumed, out int charsWritten, bool isFinalBlock)
	{
		_AssertThisNotNull();
		int num = 0;
		int num2 = 0;
		OperationStatus result2;
		while (true)
		{
			int num3;
			Rune result;
			if ((uint)source.Length > (uint)num)
			{
				char c = source[num];
				if (_asciiPreescapedData.TryGetPreescapedData(c, out var preescapedData))
				{
					if ((uint)destination.Length > (uint)num2)
					{
						destination[num2] = (char)(byte)preescapedData;
						if (((int)preescapedData & 0xFF00) == 0)
						{
							num2++;
							num++;
							continue;
						}
						preescapedData >>= 8;
						num3 = num2 + 1;
						while ((uint)destination.Length > (uint)num3)
						{
							destination[num3++] = (char)(byte)preescapedData;
							if ((byte)(preescapedData >>= 8) != 0)
							{
								continue;
							}
							goto IL_0094;
						}
					}
					goto IL_014c;
				}
				if (Rune.TryCreate(c, out result))
				{
					goto IL_00e5;
				}
				int num4 = num + 1;
				if ((uint)source.Length > (uint)num4)
				{
					if (Rune.TryCreate(c, source[num4], out result))
					{
						goto IL_00e5;
					}
				}
				else if (!isFinalBlock && char.IsHighSurrogate(c))
				{
					result2 = OperationStatus.NeedMoreData;
					break;
				}
				result = Rune.ReplacementChar;
				goto IL_0111;
			}
			result2 = OperationStatus.Done;
			break;
			IL_014c:
			result2 = OperationStatus.DestinationTooSmall;
			break;
			IL_0094:
			num2 = num3;
			num++;
			continue;
			IL_0111:
			int num5 = _scalarEscaper.EncodeUtf16(result, destination.Slice(num2));
			if (num5 >= 0)
			{
				num2 += num5;
				num += result.Utf16SequenceLength;
				continue;
			}
			goto IL_014c;
			IL_00e5:
			if (!IsScalarValueAllowed(result))
			{
				goto IL_0111;
			}
			if (result.TryEncodeToUtf16(destination.Slice(num2), out var charsWritten2))
			{
				num2 += charsWritten2;
				num += charsWritten2;
				continue;
			}
			goto IL_014c;
		}
		charsConsumed = num;
		charsWritten = num2;
		return result2;
	}

	public OperationStatus EncodeUtf8(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
	{
		_AssertThisNotNull();
		int num = 0;
		int num2 = 0;
		OperationStatus result2;
		while (true)
		{
			int num3;
			if ((uint)source.Length > (uint)num)
			{
				uint codePoint = source[num];
				if (_asciiPreescapedData.TryGetPreescapedData(codePoint, out var preescapedData))
				{
					if (BinaryPrimitives.TryWriteUInt64LittleEndian(destination.Slice(num2), preescapedData))
					{
						num2 += (int)(preescapedData >> 56);
						num++;
						continue;
					}
					num3 = num2;
					while ((uint)destination.Length > (uint)num3)
					{
						destination[num3++] = (byte)preescapedData;
						if ((byte)(preescapedData >>= 8) != 0)
						{
							continue;
						}
						goto IL_007e;
					}
				}
				else
				{
					OperationStatus operationStatus = Rune.DecodeFromUtf8(source.Slice(num), out var result, out var bytesConsumed2);
					if (operationStatus != OperationStatus.Done)
					{
						if (!isFinalBlock && operationStatus == OperationStatus.NeedMoreData)
						{
							result2 = OperationStatus.NeedMoreData;
							break;
						}
					}
					else if (IsScalarValueAllowed(result))
					{
						if (result.TryEncodeToUtf8(destination.Slice(num2), out var bytesWritten2))
						{
							num2 += bytesWritten2;
							num += bytesWritten2;
							continue;
						}
						goto IL_010b;
					}
					int num4 = _scalarEscaper.EncodeUtf8(result, destination.Slice(num2));
					if (num4 >= 0)
					{
						num2 += num4;
						num += bytesConsumed2;
						continue;
					}
				}
				goto IL_010b;
			}
			result2 = OperationStatus.Done;
			break;
			IL_007e:
			num2 = num3;
			num++;
			continue;
			IL_010b:
			result2 = OperationStatus.DestinationTooSmall;
			break;
		}
		bytesConsumed = num;
		bytesWritten = num2;
		return result2;
	}

	public int GetIndexOfFirstByteToEncode(ReadOnlySpan<byte> data)
	{
		int num = 0;
		num = data.IndexOfAnyExcept(_allowedAsciiBytes);
		if ((uint)num >= (uint)data.Length || System.Text.UnicodeUtility.IsAsciiCodePoint(data[num]))
		{
			return num;
		}
		int length = data.Length;
		data = data.Slice(num);
		Rune result;
		int bytesConsumed;
		while (!data.IsEmpty && Rune.DecodeFromUtf8(data, out result, out bytesConsumed) == OperationStatus.Done && bytesConsumed < 4 && _allowedBmpCodePoints.IsCharAllowed((char)result.Value))
		{
			data = data.Slice(bytesConsumed);
		}
		if (!data.IsEmpty)
		{
			return length - data.Length;
		}
		return -1;
	}

	public unsafe int GetIndexOfFirstCharToEncode(ReadOnlySpan<char> data)
	{
		int num = 0;
		if (data.Length >= 8)
		{
			num = data.IndexOfAnyExcept(_allowedAsciiChars);
			if ((uint)num >= (uint)data.Length || char.IsAscii(data[num]))
			{
				return num;
			}
		}
		fixed (char* ptr = data)
		{
			nuint num2 = (uint)data.Length;
			nuint num3 = (uint)num;
			if (num3 < num2)
			{
				_AssertThisNotNull();
				nint num4 = 0;
				while (true)
				{
					if (num2 - num3 >= 8)
					{
						num4 = -1;
						if (_allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]) && _allowedBmpCodePoints.IsCharAllowed(ptr[(nuint)((nint)num3 + ++num4)]))
						{
							num3 += 8;
							continue;
						}
						num3 += (nuint)num4;
						break;
					}
					for (; num3 < num2 && _allowedBmpCodePoints.IsCharAllowed(ptr[num3]); num3++)
					{
					}
					break;
				}
			}
			int num5 = (int)num3;
			if (num5 == (int)num2)
			{
				num5 = -1;
			}
			return num5;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsScalarValueAllowed(Rune value)
	{
		return _allowedBmpCodePoints.IsCodePointAllowed((uint)value.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void _AssertThisNotNull()
	{
		_ = GetType() == typeof(OptimizedInboxTextEncoder);
	}
}
