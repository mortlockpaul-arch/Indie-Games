using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Text.Json;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public ref struct Utf8JsonReader
{
	private readonly struct PartialStateForRollback(long totalConsumed, long bytePositionInLine, int consumed, SequencePosition currentPosition)
	{
		public readonly long _prevTotalConsumed = totalConsumed;

		public readonly long _prevBytePositionInLine = bytePositionInLine;

		public readonly int _prevConsumed = consumed;

		public readonly SequencePosition _prevCurrentPosition = currentPosition;

		public SequencePosition GetStartPosition(int offset = 0)
		{
			return new SequencePosition(_prevCurrentPosition.GetObject(), _prevCurrentPosition.GetInteger() + _prevConsumed + offset);
		}
	}

	private ReadOnlySpan<byte> _buffer;

	private readonly bool _isFinalBlock;

	private readonly bool _isInputSequence;

	private long _lineNumber;

	private long _bytePositionInLine;

	private int _consumed;

	private bool _inObject;

	private bool _isNotPrimitive;

	private JsonTokenType _tokenType;

	private JsonTokenType _previousTokenType;

	private JsonReaderOptions _readerOptions;

	private BitStack _bitStack;

	private long _totalConsumed;

	private bool _isLastSegment;

	private readonly bool _isMultiSegment;

	private bool _trailingCommaBeforeComment;

	private SequencePosition _nextPosition;

	private SequencePosition _currentPosition;

	private readonly ReadOnlySequence<byte> _sequence;

	private readonly bool IsLastSpan
	{
		get
		{
			if (_isFinalBlock)
			{
				if (_isMultiSegment)
				{
					return _isLastSegment;
				}
				return true;
			}
			return false;
		}
	}

	internal readonly ReadOnlySequence<byte> OriginalSequence => _sequence;

	internal readonly ReadOnlySpan<byte> OriginalSpan
	{
		get
		{
			if (!_sequence.IsEmpty)
			{
				return default(ReadOnlySpan<byte>);
			}
			return _buffer;
		}
	}

	internal readonly int ValueLength
	{
		get
		{
			if (!HasValueSequence)
			{
				return ValueSpan.Length;
			}
			return checked((int)ValueSequence.Length);
		}
	}

	internal readonly bool AllowMultipleValues => _readerOptions.AllowMultipleValues;

	public ReadOnlySpan<byte> ValueSpan { get; private set; }

	public readonly long BytesConsumed => _totalConsumed + _consumed;

	public long TokenStartIndex { get; private set; }

	public readonly int CurrentDepth
	{
		get
		{
			int num = _bitStack.CurrentDepth;
			JsonTokenType tokenType = TokenType;
			if ((tokenType == JsonTokenType.StartObject || tokenType == JsonTokenType.StartArray) ? true : false)
			{
				num--;
			}
			return num;
		}
	}

	internal readonly bool IsInArray => !_inObject;

	public readonly JsonTokenType TokenType => _tokenType;

	public bool HasValueSequence { get; private set; }

	public bool ValueIsEscaped { get; private set; }

	public readonly bool IsFinalBlock => _isFinalBlock;

	public ReadOnlySequence<byte> ValueSequence { get; private set; }

	public readonly SequencePosition Position
	{
		get
		{
			if (_isInputSequence)
			{
				return _sequence.GetPosition(_consumed, _currentPosition);
			}
			return default(SequencePosition);
		}
	}

	public readonly JsonReaderState CurrentState => new JsonReaderState(_lineNumber, _bytePositionInLine, _inObject, _isNotPrimitive, ValueIsEscaped, _trailingCommaBeforeComment, _tokenType, _previousTokenType, _readerOptions, _bitStack);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"TokenType = {DebugTokenType}, TokenStartIndex = {TokenStartIndex}, Consumed = {BytesConsumed}";

	private string DebugTokenType => TokenType switch
	{
		JsonTokenType.Comment => "Comment", 
		JsonTokenType.EndArray => "EndArray", 
		JsonTokenType.EndObject => "EndObject", 
		JsonTokenType.False => "False", 
		JsonTokenType.None => "None", 
		JsonTokenType.Null => "Null", 
		JsonTokenType.Number => "Number", 
		JsonTokenType.PropertyName => "PropertyName", 
		JsonTokenType.StartArray => "StartArray", 
		JsonTokenType.StartObject => "StartObject", 
		JsonTokenType.String => "String", 
		JsonTokenType.True => "True", 
		_ => ((byte)TokenType).ToString(), 
	};

	public Utf8JsonReader(ReadOnlySpan<byte> jsonData, bool isFinalBlock, JsonReaderState state)
	{
		_buffer = jsonData;
		_isFinalBlock = isFinalBlock;
		_isInputSequence = false;
		_lineNumber = state._lineNumber;
		_bytePositionInLine = state._bytePositionInLine;
		_inObject = state._inObject;
		_isNotPrimitive = state._isNotPrimitive;
		ValueIsEscaped = state._valueIsEscaped;
		_trailingCommaBeforeComment = state._trailingCommaBeforeComment;
		_tokenType = state._tokenType;
		_previousTokenType = state._previousTokenType;
		_readerOptions = state._readerOptions;
		if (_readerOptions.MaxDepth == 0)
		{
			_readerOptions.MaxDepth = 64;
		}
		_bitStack = state._bitStack;
		_consumed = 0;
		TokenStartIndex = 0L;
		_totalConsumed = 0L;
		_isLastSegment = _isFinalBlock;
		_isMultiSegment = false;
		ValueSpan = ReadOnlySpan<byte>.Empty;
		_currentPosition = default(SequencePosition);
		_nextPosition = default(SequencePosition);
		_sequence = default(ReadOnlySequence<byte>);
		HasValueSequence = false;
		ValueSequence = ReadOnlySequence<byte>.Empty;
	}

	public Utf8JsonReader(ReadOnlySpan<byte> jsonData, JsonReaderOptions options = default(JsonReaderOptions))
		: this(jsonData, isFinalBlock: true, new JsonReaderState(options))
	{
	}

	public bool Read()
	{
		bool num = (_isMultiSegment ? ReadMultiSegment() : ReadSingleSegment());
		if (!num && _isFinalBlock && TokenType == JsonTokenType.None && !_readerOptions.AllowMultipleValues)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedJsonTokens, 0);
		}
		return num;
	}

	public void Skip()
	{
		if (!_isFinalBlock)
		{
			ThrowHelper.ThrowInvalidOperationException_CannotSkipOnPartial();
		}
		SkipHelper();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SkipHelper()
	{
		if (TokenType == JsonTokenType.PropertyName)
		{
			Read();
		}
		JsonTokenType tokenType = TokenType;
		if ((tokenType == JsonTokenType.StartObject || tokenType == JsonTokenType.StartArray) ? true : false)
		{
			int currentDepth = CurrentDepth;
			do
			{
				Read();
			}
			while (currentDepth < CurrentDepth);
		}
	}

	public bool TrySkip()
	{
		if (_isFinalBlock)
		{
			SkipHelper();
			return true;
		}
		Utf8JsonReader utf8JsonReader = this;
		bool num = TrySkipPartial(CurrentDepth);
		if (!num)
		{
			this = utf8JsonReader;
		}
		return num;
	}

	internal bool TrySkipPartial(int targetDepth)
	{
		if (targetDepth == CurrentDepth)
		{
			if (TokenType == JsonTokenType.PropertyName && !Read())
			{
				return false;
			}
			JsonTokenType tokenType = TokenType;
			if ((tokenType != JsonTokenType.StartObject && tokenType != JsonTokenType.StartArray) || 1 == 0)
			{
				return true;
			}
		}
		do
		{
			if (!Read())
			{
				return false;
			}
		}
		while (targetDepth < CurrentDepth);
		return true;
	}

	public readonly bool ValueTextEquals(ReadOnlySpan<byte> utf8Text)
	{
		if (!IsTokenTypeString(TokenType))
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedStringComparison(TokenType);
		}
		return TextEqualsHelper(utf8Text);
	}

	public readonly bool ValueTextEquals(string? text)
	{
		return ValueTextEquals(text.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly bool TextEqualsHelper(ReadOnlySpan<byte> otherUtf8Text)
	{
		if (HasValueSequence)
		{
			return CompareToSequence(otherUtf8Text);
		}
		if (ValueIsEscaped)
		{
			return UnescapeAndCompare(otherUtf8Text);
		}
		return otherUtf8Text.SequenceEqual(ValueSpan);
	}

	public readonly bool ValueTextEquals(ReadOnlySpan<char> text)
	{
		if (!IsTokenTypeString(TokenType))
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedStringComparison(TokenType);
		}
		if (MatchNotPossible(text.Length))
		{
			return false;
		}
		byte[] array = null;
		int num = checked(text.Length * 3);
		Span<byte> destination;
		if (num > 256)
		{
			array = ArrayPool<byte>.Shared.Rent(num);
			destination = array;
		}
		else
		{
			destination = stackalloc byte[256];
		}
		bool result = JsonWriterHelper.ToUtf8(text, destination, out var written) != OperationStatus.InvalidData && TextEqualsHelper(destination.Slice(0, written));
		if (array != null)
		{
			destination.Slice(0, written).Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	private readonly bool CompareToSequence(ReadOnlySpan<byte> other)
	{
		if (ValueIsEscaped)
		{
			return UnescapeSequenceAndCompare(other);
		}
		ReadOnlySequence<byte> valueSequence = ValueSequence;
		if (valueSequence.Length != other.Length)
		{
			return false;
		}
		int num = 0;
		foreach (ReadOnlyMemory<byte> item in valueSequence)
		{
			ReadOnlySpan<byte> span = item.Span;
			if (other.Slice(num).StartsWith(span))
			{
				num += span.Length;
				continue;
			}
			return false;
		}
		return true;
	}

	private readonly bool UnescapeAndCompare(ReadOnlySpan<byte> other)
	{
		ReadOnlySpan<byte> valueSpan = ValueSpan;
		if (valueSpan.Length < other.Length || valueSpan.Length / 6 > other.Length)
		{
			return false;
		}
		int num = valueSpan.IndexOf((byte)92);
		if (!other.StartsWith(valueSpan.Slice(0, num)))
		{
			return false;
		}
		return JsonReaderHelper.UnescapeAndCompare(valueSpan.Slice(num), other.Slice(num));
	}

	private readonly bool UnescapeSequenceAndCompare(ReadOnlySpan<byte> other)
	{
		ReadOnlySequence<byte> valueSequence = ValueSequence;
		long length = valueSequence.Length;
		if (length < other.Length || length / 6 > other.Length)
		{
			return false;
		}
		int num = 0;
		bool result = false;
		foreach (ReadOnlyMemory<byte> item in valueSequence)
		{
			ReadOnlySpan<byte> span = item.Span;
			int num2 = span.IndexOf((byte)92);
			if (num2 != -1)
			{
				if (other.Slice(num).StartsWith(span.Slice(0, num2)))
				{
					num += num2;
					other = other.Slice(num);
					valueSequence = valueSequence.Slice(num);
					result = ((!valueSequence.IsSingleSegment) ? JsonReaderHelper.UnescapeAndCompare(valueSequence, other) : JsonReaderHelper.UnescapeAndCompare(valueSequence.First.Span, other));
				}
				break;
			}
			if (!other.Slice(num).StartsWith(span))
			{
				break;
			}
			num += span.Length;
		}
		return result;
	}

	private static bool IsTokenTypeString(JsonTokenType tokenType)
	{
		if (tokenType != JsonTokenType.PropertyName)
		{
			return tokenType == JsonTokenType.String;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly bool MatchNotPossible(int charTextLength)
	{
		if (HasValueSequence)
		{
			return MatchNotPossibleSequence(charTextLength);
		}
		int length = ValueSpan.Length;
		if (length < charTextLength || length / (ValueIsEscaped ? 6 : 3) > charTextLength)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private readonly bool MatchNotPossibleSequence(int charTextLength)
	{
		long length = ValueSequence.Length;
		if (length < charTextLength || length / (ValueIsEscaped ? 6 : 3) > charTextLength)
		{
			return true;
		}
		return false;
	}

	private void StartObject()
	{
		if (_bitStack.CurrentDepth >= _readerOptions.MaxDepth)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ObjectDepthTooLarge, 0);
		}
		_bitStack.PushTrue();
		ValueSpan = _buffer.Slice(_consumed, 1);
		_consumed++;
		_bytePositionInLine++;
		_tokenType = JsonTokenType.StartObject;
		_inObject = true;
	}

	private void EndObject()
	{
		if (!_inObject || _bitStack.CurrentDepth <= 0)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.MismatchedObjectArray, 125);
		}
		if (_trailingCommaBeforeComment)
		{
			if (!_readerOptions.AllowTrailingCommas)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
			}
			_trailingCommaBeforeComment = false;
		}
		_tokenType = JsonTokenType.EndObject;
		ValueSpan = _buffer.Slice(_consumed, 1);
		UpdateBitStackOnEndToken();
	}

	private void StartArray()
	{
		if (_bitStack.CurrentDepth >= _readerOptions.MaxDepth)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ArrayDepthTooLarge, 0);
		}
		_bitStack.PushFalse();
		ValueSpan = _buffer.Slice(_consumed, 1);
		_consumed++;
		_bytePositionInLine++;
		_tokenType = JsonTokenType.StartArray;
		_inObject = false;
	}

	private void EndArray()
	{
		if (_inObject || _bitStack.CurrentDepth <= 0)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.MismatchedObjectArray, 93);
		}
		if (_trailingCommaBeforeComment)
		{
			if (!_readerOptions.AllowTrailingCommas)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
			}
			_trailingCommaBeforeComment = false;
		}
		_tokenType = JsonTokenType.EndArray;
		ValueSpan = _buffer.Slice(_consumed, 1);
		UpdateBitStackOnEndToken();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateBitStackOnEndToken()
	{
		_consumed++;
		_bytePositionInLine++;
		_inObject = _bitStack.Pop();
	}

	private bool ReadSingleSegment()
	{
		bool flag = false;
		ValueSpan = default(ReadOnlySpan<byte>);
		ValueIsEscaped = false;
		if (HasMoreData())
		{
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpace();
				if (!HasMoreData())
				{
					goto IL_0139;
				}
				b = _buffer[_consumed];
			}
			TokenStartIndex = _consumed;
			if (_tokenType != JsonTokenType.None)
			{
				if (b == 47)
				{
					flag = ConsumeNextTokenOrRollback(b);
				}
				else if (_tokenType == JsonTokenType.StartObject)
				{
					if (b == 125)
					{
						EndObject();
						goto IL_0137;
					}
					if (b != 34)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
					}
					int consumed = _consumed;
					long bytePositionInLine = _bytePositionInLine;
					long lineNumber = _lineNumber;
					flag = ConsumePropertyName();
					if (!flag)
					{
						_consumed = consumed;
						_tokenType = JsonTokenType.StartObject;
						_bytePositionInLine = bytePositionInLine;
						_lineNumber = lineNumber;
					}
				}
				else if (_tokenType != JsonTokenType.StartArray)
				{
					flag = ((_tokenType != JsonTokenType.PropertyName) ? ConsumeNextTokenOrRollback(b) : ConsumeValue(b));
				}
				else
				{
					if (b == 93)
					{
						EndArray();
						goto IL_0137;
					}
					flag = ConsumeValue(b);
				}
			}
			else
			{
				flag = ReadFirstToken(b);
			}
		}
		goto IL_0139;
		IL_0139:
		return flag;
		IL_0137:
		flag = true;
		goto IL_0139;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasMoreData()
	{
		if (_consumed >= (uint)_buffer.Length)
		{
			if (_isNotPrimitive && IsLastSpan)
			{
				if (_bitStack.CurrentDepth != 0)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ZeroDepthAtEnd, 0);
				}
				if (_readerOptions.CommentHandling == JsonCommentHandling.Allow && _tokenType == JsonTokenType.Comment)
				{
					return false;
				}
				JsonTokenType tokenType = _tokenType;
				if (tokenType != JsonTokenType.EndArray && tokenType != JsonTokenType.EndObject)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJsonNonPrimitive, 0);
				}
			}
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasMoreData(ExceptionResource resource)
	{
		if (_consumed >= (uint)_buffer.Length)
		{
			if (IsLastSpan)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, resource, 0);
			}
			return false;
		}
		return true;
	}

	private bool ReadFirstToken(byte first)
	{
		switch (first)
		{
		case 123:
			_bitStack.SetFirstBit();
			_tokenType = JsonTokenType.StartObject;
			ValueSpan = _buffer.Slice(_consumed, 1);
			_consumed++;
			_bytePositionInLine++;
			_inObject = true;
			_isNotPrimitive = true;
			break;
		case 91:
			_bitStack.ResetFirstBit();
			_tokenType = JsonTokenType.StartArray;
			ValueSpan = _buffer.Slice(_consumed, 1);
			_consumed++;
			_bytePositionInLine++;
			_isNotPrimitive = true;
			break;
		default:
		{
			ReadOnlySpan<byte> buffer = _buffer;
			if (JsonHelpers.IsDigit(first) || first == 45)
			{
				if (!TryGetNumber(buffer.Slice(_consumed), out var consumed))
				{
					return false;
				}
				_tokenType = JsonTokenType.Number;
				_consumed += consumed;
				_bytePositionInLine += consumed;
			}
			else if (!ConsumeValue(first))
			{
				return false;
			}
			JsonTokenType tokenType = _tokenType;
			bool isNotPrimitive = ((tokenType == JsonTokenType.StartObject || tokenType == JsonTokenType.StartArray) ? true : false);
			_isNotPrimitive = isNotPrimitive;
			break;
		}
		}
		return true;
	}

	private void SkipWhiteSpace()
	{
		ReadOnlySpan<byte> buffer = _buffer;
		while (_consumed < buffer.Length)
		{
			byte b = buffer[_consumed];
			if (b == 32 || b == 13 || b == 10 || b == 9)
			{
				if (b == 10)
				{
					_lineNumber++;
					_bytePositionInLine = 0L;
				}
				else
				{
					_bytePositionInLine++;
				}
				_consumed++;
				continue;
			}
			break;
		}
	}

	private bool ConsumeValue(byte marker)
	{
		while (true)
		{
			_trailingCommaBeforeComment = false;
			switch (marker)
			{
			case 34:
				return ConsumeString();
			case 123:
				StartObject();
				break;
			case 91:
				StartArray();
				break;
			default:
				if (JsonHelpers.IsDigit(marker) || marker == 45)
				{
					return ConsumeNumber();
				}
				switch (marker)
				{
				case 102:
					return ConsumeLiteral(JsonConstants.FalseValue, JsonTokenType.False);
				case 116:
					return ConsumeLiteral(JsonConstants.TrueValue, JsonTokenType.True);
				case 110:
					return ConsumeLiteral(JsonConstants.NullValue, JsonTokenType.Null);
				}
				switch (_readerOptions.CommentHandling)
				{
				case JsonCommentHandling.Allow:
					if (marker == 47)
					{
						return ConsumeComment();
					}
					break;
				default:
					if (marker != 47)
					{
						break;
					}
					if (SkipComment())
					{
						if (_consumed >= (uint)_buffer.Length)
						{
							if (_isNotPrimitive && IsLastSpan && _tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJsonNonPrimitive, 0);
							}
							return false;
						}
						marker = _buffer[_consumed];
						if (marker <= 32)
						{
							SkipWhiteSpace();
							if (!HasMoreData())
							{
								return false;
							}
							marker = _buffer[_consumed];
						}
						goto IL_0140;
					}
					return false;
				case JsonCommentHandling.Disallow:
					break;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, marker);
				break;
			}
			break;
			IL_0140:
			TokenStartIndex = _consumed;
		}
		return true;
	}

	private bool ConsumeLiteral(ReadOnlySpan<byte> literal, JsonTokenType tokenType)
	{
		ReadOnlySpan<byte> span = _buffer.Slice(_consumed);
		if (!span.StartsWith(literal))
		{
			return CheckLiteral(span, literal);
		}
		ValueSpan = span.Slice(0, literal.Length);
		_tokenType = tokenType;
		_consumed += literal.Length;
		_bytePositionInLine += literal.Length;
		return true;
	}

	private bool CheckLiteral(ReadOnlySpan<byte> span, ReadOnlySpan<byte> literal)
	{
		int num = 0;
		for (int i = 1; i < literal.Length; i++)
		{
			if (span.Length > i)
			{
				if (span[i] != literal[i])
				{
					_bytePositionInLine += i;
					ThrowInvalidLiteral(span);
				}
				continue;
			}
			num = i;
			break;
		}
		if (IsLastSpan)
		{
			_bytePositionInLine += num;
			ThrowInvalidLiteral(span);
		}
		return false;
	}

	private void ThrowInvalidLiteral(ReadOnlySpan<byte> span)
	{
		ThrowHelper.ThrowJsonReaderException(ref this, span[0] switch
		{
			116 => ExceptionResource.ExpectedTrue, 
			102 => ExceptionResource.ExpectedFalse, 
			_ => ExceptionResource.ExpectedNull, 
		}, 0, span);
	}

	private bool ConsumeNumber()
	{
		if (!TryGetNumber(_buffer.Slice(_consumed), out var consumed))
		{
			return false;
		}
		_tokenType = JsonTokenType.Number;
		_consumed += consumed;
		_bytePositionInLine += consumed;
		if (_consumed >= (uint)_buffer.Length && _isNotPrimitive)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, _buffer[_consumed - 1]);
		}
		return true;
	}

	private bool ConsumePropertyName()
	{
		_trailingCommaBeforeComment = false;
		if (!ConsumeString())
		{
			return false;
		}
		if (!HasMoreData(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
		{
			return false;
		}
		byte b = _buffer[_consumed];
		if (b <= 32)
		{
			SkipWhiteSpace();
			if (!HasMoreData(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
			{
				return false;
			}
			b = _buffer[_consumed];
		}
		if (b != 58)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedSeparatorAfterPropertyNameNotFound, b);
		}
		_consumed++;
		_bytePositionInLine++;
		_tokenType = JsonTokenType.PropertyName;
		return true;
	}

	private bool ConsumeString()
	{
		ReadOnlySpan<byte> readOnlySpan = _buffer.Slice(_consumed + 1);
		int num = readOnlySpan.IndexOfQuoteOrAnyControlOrBackSlash();
		if (num >= 0)
		{
			if (readOnlySpan[num] == 34)
			{
				_bytePositionInLine += num + 2;
				ValueSpan = readOnlySpan.Slice(0, num);
				ValueIsEscaped = false;
				_tokenType = JsonTokenType.String;
				_consumed += num + 2;
				return true;
			}
			return ConsumeStringAndValidate(readOnlySpan, num);
		}
		if (IsLastSpan)
		{
			_bytePositionInLine += readOnlySpan.Length + 1;
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
		}
		return false;
	}

	private bool ConsumeStringAndValidate(ReadOnlySpan<byte> data, int idx)
	{
		long bytePositionInLine = _bytePositionInLine;
		long lineNumber = _lineNumber;
		_bytePositionInLine += idx + 1;
		bool flag = false;
		while (true)
		{
			if (idx < data.Length)
			{
				byte b = data[idx];
				if (b == 34)
				{
					if (!flag)
					{
						break;
					}
					flag = false;
				}
				else if (b == 92)
				{
					flag = !flag;
				}
				else if (flag)
				{
					if (JsonConstants.EscapableChars.IndexOf(b) == -1)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterAfterEscapeWithinString, b);
					}
					if (b == 117)
					{
						_bytePositionInLine++;
						if (!ValidateHexDigits(data, idx + 1))
						{
							idx = data.Length;
							goto IL_00de;
						}
						idx += 4;
					}
					flag = false;
				}
				else if (b < 32)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, b);
				}
				_bytePositionInLine++;
				idx++;
				continue;
			}
			goto IL_00de;
			IL_00de:
			if (idx < data.Length)
			{
				break;
			}
			if (IsLastSpan)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
			}
			_lineNumber = lineNumber;
			_bytePositionInLine = bytePositionInLine;
			return false;
		}
		_bytePositionInLine++;
		ValueSpan = data.Slice(0, idx);
		ValueIsEscaped = true;
		_tokenType = JsonTokenType.String;
		_consumed += idx + 2;
		return true;
	}

	private bool ValidateHexDigits(ReadOnlySpan<byte> data, int idx)
	{
		for (int i = idx; i < data.Length; i++)
		{
			byte nextByte = data[i];
			if (!JsonReaderHelper.IsHexDigit(nextByte))
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidHexCharacterWithinString, nextByte);
			}
			if (i - idx >= 3)
			{
				return true;
			}
			_bytePositionInLine++;
		}
		return false;
	}

	private bool TryGetNumber(ReadOnlySpan<byte> data, out int consumed)
	{
		consumed = 0;
		int i = 0;
		if (ConsumeNegativeSign(ref data, ref i) == ConsumeNumberResult.NeedMoreData)
		{
			return false;
		}
		byte b = data[i];
		if (b == 48)
		{
			ConsumeNumberResult consumeNumberResult = ConsumeZero(ref data, ref i);
			if (consumeNumberResult == ConsumeNumberResult.NeedMoreData)
			{
				return false;
			}
			if (consumeNumberResult != ConsumeNumberResult.Success)
			{
				b = data[i];
				goto IL_009e;
			}
		}
		else
		{
			i++;
			ConsumeNumberResult consumeNumberResult2 = ConsumeIntegerDigits(ref data, ref i);
			if (consumeNumberResult2 == ConsumeNumberResult.NeedMoreData)
			{
				return false;
			}
			if (consumeNumberResult2 != ConsumeNumberResult.Success)
			{
				b = data[i];
				if (b != 46 && b != 69 && b != 101)
				{
					_bytePositionInLine += i;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, b);
				}
				goto IL_009e;
			}
		}
		goto IL_014b;
		IL_009e:
		if (b == 46)
		{
			i++;
			ConsumeNumberResult consumeNumberResult3 = ConsumeDecimalDigits(ref data, ref i);
			if (consumeNumberResult3 == ConsumeNumberResult.NeedMoreData)
			{
				return false;
			}
			if (consumeNumberResult3 == ConsumeNumberResult.Success)
			{
				goto IL_014b;
			}
			b = data[i];
			if (b != 69 && b != 101)
			{
				_bytePositionInLine += i;
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedNextDigitEValueNotFound, b);
			}
		}
		i++;
		if (ConsumeSign(ref data, ref i) == ConsumeNumberResult.NeedMoreData)
		{
			return false;
		}
		i++;
		switch (ConsumeIntegerDigits(ref data, ref i))
		{
		case ConsumeNumberResult.NeedMoreData:
			return false;
		default:
			_bytePositionInLine += i;
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, data[i]);
			break;
		case ConsumeNumberResult.Success:
			break;
		}
		goto IL_014b;
		IL_014b:
		ValueSpan = data.Slice(0, i);
		consumed = i;
		return true;
	}

	private ConsumeNumberResult ConsumeNegativeSign(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		byte b = data[i];
		if (b == 45)
		{
			i++;
			if (i >= data.Length)
			{
				if (IsLastSpan)
				{
					_bytePositionInLine += i;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				return ConsumeNumberResult.NeedMoreData;
			}
			b = data[i];
			if (!JsonHelpers.IsDigit(b))
			{
				_bytePositionInLine += i;
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterSign, b);
			}
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private ConsumeNumberResult ConsumeZero(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		i++;
		if (i < data.Length)
		{
			byte value = data[i];
			if (JsonConstants.Delimiters.IndexOf(value) >= 0)
			{
				return ConsumeNumberResult.Success;
			}
			value = data[i];
			if (value != 46 && value != 69 && value != 101)
			{
				_bytePositionInLine += i;
				ThrowHelper.ThrowJsonReaderException(ref this, JsonHelpers.IsInRangeInclusive(value, 48, 57) ? ExceptionResource.InvalidLeadingZeroInNumber : ExceptionResource.ExpectedEndOfDigitNotFound, value);
			}
			return ConsumeNumberResult.OperationIncomplete;
		}
		if (IsLastSpan)
		{
			return ConsumeNumberResult.Success;
		}
		return ConsumeNumberResult.NeedMoreData;
	}

	private ConsumeNumberResult ConsumeIntegerDigits(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		byte value = 0;
		while (i < data.Length)
		{
			value = data[i];
			if (!JsonHelpers.IsDigit(value))
			{
				break;
			}
			i++;
		}
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				return ConsumeNumberResult.Success;
			}
			return ConsumeNumberResult.NeedMoreData;
		}
		if (JsonConstants.Delimiters.IndexOf(value) >= 0)
		{
			return ConsumeNumberResult.Success;
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private ConsumeNumberResult ConsumeDecimalDigits(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				_bytePositionInLine += i;
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
			}
			return ConsumeNumberResult.NeedMoreData;
		}
		byte b = data[i];
		if (!JsonHelpers.IsDigit(b))
		{
			_bytePositionInLine += i;
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterDecimal, b);
		}
		i++;
		return ConsumeIntegerDigits(ref data, ref i);
	}

	private ConsumeNumberResult ConsumeSign(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				_bytePositionInLine += i;
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
			}
			return ConsumeNumberResult.NeedMoreData;
		}
		byte b = data[i];
		if (b == 43 || b == 45)
		{
			i++;
			if (i >= data.Length)
			{
				if (IsLastSpan)
				{
					_bytePositionInLine += i;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				return ConsumeNumberResult.NeedMoreData;
			}
			b = data[i];
		}
		if (!JsonHelpers.IsDigit(b))
		{
			_bytePositionInLine += i;
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterSign, b);
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private bool ConsumeNextTokenOrRollback(byte marker)
	{
		int consumed = _consumed;
		long bytePositionInLine = _bytePositionInLine;
		long lineNumber = _lineNumber;
		JsonTokenType tokenType = _tokenType;
		bool trailingCommaBeforeComment = _trailingCommaBeforeComment;
		switch (ConsumeNextToken(marker))
		{
		case ConsumeTokenResult.Success:
			return true;
		case ConsumeTokenResult.NotEnoughDataRollBackState:
			_consumed = consumed;
			_tokenType = tokenType;
			_bytePositionInLine = bytePositionInLine;
			_lineNumber = lineNumber;
			_trailingCommaBeforeComment = trailingCommaBeforeComment;
			break;
		}
		return false;
	}

	private ConsumeTokenResult ConsumeNextToken(byte marker)
	{
		if (_readerOptions.CommentHandling != JsonCommentHandling.Disallow)
		{
			if (_readerOptions.CommentHandling != JsonCommentHandling.Allow)
			{
				return ConsumeNextTokenUntilAfterAllCommentsAreSkipped(marker);
			}
			if (marker == 47)
			{
				if (!ConsumeComment())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (_tokenType == JsonTokenType.Comment)
			{
				return ConsumeNextTokenFromLastNonCommentToken();
			}
		}
		if (_bitStack.CurrentDepth == 0)
		{
			if (_readerOptions.AllowMultipleValues)
			{
				if (!ReadFirstToken(marker))
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, marker);
		}
		switch (marker)
		{
		case 44:
		{
			_consumed++;
			_bytePositionInLine++;
			if (_consumed >= (uint)_buffer.Length)
			{
				if (IsLastSpan)
				{
					_consumed--;
					_bytePositionInLine--;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
				}
				return ConsumeTokenResult.NotEnoughDataRollBackState;
			}
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpace();
				if (!HasMoreData(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				b = _buffer[_consumed];
			}
			TokenStartIndex = _consumed;
			if (_readerOptions.CommentHandling == JsonCommentHandling.Allow && b == 47)
			{
				_trailingCommaBeforeComment = true;
				if (!ConsumeComment())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (_inObject)
			{
				if (b != 34)
				{
					if (b == 125)
					{
						if (_readerOptions.AllowTrailingCommas)
						{
							EndObject();
							return ConsumeTokenResult.Success;
						}
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
					}
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
				}
				if (!ConsumePropertyName())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (b == 93)
			{
				if (_readerOptions.AllowTrailingCommas)
				{
					EndArray();
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
			}
			if (!ConsumeValue(b))
			{
				return ConsumeTokenResult.NotEnoughDataRollBackState;
			}
			return ConsumeTokenResult.Success;
		}
		case 125:
			EndObject();
			break;
		case 93:
			EndArray();
			break;
		default:
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.FoundInvalidCharacter, marker);
			break;
		}
		return ConsumeTokenResult.Success;
	}

	private ConsumeTokenResult ConsumeNextTokenFromLastNonCommentToken()
	{
		if (JsonReaderHelper.IsTokenTypePrimitive(_previousTokenType))
		{
			_tokenType = (_inObject ? JsonTokenType.StartObject : JsonTokenType.StartArray);
		}
		else
		{
			_tokenType = _previousTokenType;
		}
		if (HasMoreData())
		{
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpace();
				if (!HasMoreData())
				{
					goto IL_0363;
				}
				b = _buffer[_consumed];
			}
			if (_bitStack.CurrentDepth == 0 && _tokenType != JsonTokenType.None)
			{
				if (_readerOptions.AllowMultipleValues)
				{
					if (!ReadFirstToken(b))
					{
						return ConsumeTokenResult.NotEnoughDataRollBackState;
					}
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, b);
			}
			TokenStartIndex = _consumed;
			if (b != 44)
			{
				if (b == 125)
				{
					EndObject();
				}
				else
				{
					if (b != 93)
					{
						if (_tokenType == JsonTokenType.None)
						{
							if (ReadFirstToken(b))
							{
								goto IL_0361;
							}
						}
						else if (_tokenType == JsonTokenType.StartObject)
						{
							if (b != 34)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
							}
							int consumed = _consumed;
							long bytePositionInLine = _bytePositionInLine;
							long lineNumber = _lineNumber;
							if (ConsumePropertyName())
							{
								goto IL_0361;
							}
							_consumed = consumed;
							_tokenType = JsonTokenType.StartObject;
							_bytePositionInLine = bytePositionInLine;
							_lineNumber = lineNumber;
						}
						else if (_tokenType == JsonTokenType.StartArray)
						{
							if (ConsumeValue(b))
							{
								goto IL_0361;
							}
						}
						else if (_tokenType == JsonTokenType.PropertyName)
						{
							if (ConsumeValue(b))
							{
								goto IL_0361;
							}
						}
						else if (_inObject)
						{
							if (b != 34)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
							}
							if (ConsumePropertyName())
							{
								goto IL_0361;
							}
						}
						else if (ConsumeValue(b))
						{
							goto IL_0361;
						}
						goto IL_0363;
					}
					EndArray();
				}
				goto IL_0361;
			}
			if ((int)_previousTokenType <= 1 || _previousTokenType == JsonTokenType.StartArray || _trailingCommaBeforeComment)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueAfterComment, b);
			}
			_consumed++;
			_bytePositionInLine++;
			if (_consumed >= (uint)_buffer.Length)
			{
				if (IsLastSpan)
				{
					_consumed--;
					_bytePositionInLine--;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
				}
			}
			else
			{
				b = _buffer[_consumed];
				if (b <= 32)
				{
					SkipWhiteSpace();
					if (!HasMoreData(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
					{
						goto IL_0363;
					}
					b = _buffer[_consumed];
				}
				TokenStartIndex = _consumed;
				if (b == 47)
				{
					_trailingCommaBeforeComment = true;
					if (ConsumeComment())
					{
						goto IL_0361;
					}
				}
				else if (_inObject)
				{
					if (b != 34)
					{
						if (b == 125)
						{
							if (_readerOptions.AllowTrailingCommas)
							{
								EndObject();
								goto IL_0361;
							}
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
						}
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
					}
					if (ConsumePropertyName())
					{
						goto IL_0361;
					}
				}
				else
				{
					if (b == 93)
					{
						if (_readerOptions.AllowTrailingCommas)
						{
							EndArray();
							goto IL_0361;
						}
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
					}
					if (ConsumeValue(b))
					{
						goto IL_0361;
					}
				}
			}
		}
		goto IL_0363;
		IL_0363:
		return ConsumeTokenResult.NotEnoughDataRollBackState;
		IL_0361:
		return ConsumeTokenResult.Success;
	}

	private bool SkipAllComments(scoped ref byte marker)
	{
		while (true)
		{
			if (marker == 47)
			{
				if (!SkipComment() || !HasMoreData())
				{
					break;
				}
				marker = _buffer[_consumed];
				if (marker <= 32)
				{
					SkipWhiteSpace();
					if (!HasMoreData())
					{
						break;
					}
					marker = _buffer[_consumed];
				}
				continue;
			}
			return true;
		}
		return false;
	}

	private bool SkipAllComments(scoped ref byte marker, ExceptionResource resource)
	{
		while (true)
		{
			if (marker == 47)
			{
				if (!SkipComment() || !HasMoreData(resource))
				{
					break;
				}
				marker = _buffer[_consumed];
				if (marker <= 32)
				{
					SkipWhiteSpace();
					if (!HasMoreData(resource))
					{
						break;
					}
					marker = _buffer[_consumed];
				}
				continue;
			}
			return true;
		}
		return false;
	}

	private ConsumeTokenResult ConsumeNextTokenUntilAfterAllCommentsAreSkipped(byte marker)
	{
		if (SkipAllComments(ref marker))
		{
			TokenStartIndex = _consumed;
			if (_tokenType == JsonTokenType.StartObject)
			{
				if (marker == 125)
				{
					EndObject();
				}
				else
				{
					if (marker != 34)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, marker);
					}
					int consumed = _consumed;
					long bytePositionInLine = _bytePositionInLine;
					long lineNumber = _lineNumber;
					if (!ConsumePropertyName())
					{
						_consumed = consumed;
						_tokenType = JsonTokenType.StartObject;
						_bytePositionInLine = bytePositionInLine;
						_lineNumber = lineNumber;
						goto IL_02a0;
					}
				}
			}
			else if (_tokenType == JsonTokenType.StartArray)
			{
				if (marker == 93)
				{
					EndArray();
				}
				else if (!ConsumeValue(marker))
				{
					goto IL_02a0;
				}
			}
			else if (_tokenType == JsonTokenType.PropertyName)
			{
				if (!ConsumeValue(marker))
				{
					goto IL_02a0;
				}
			}
			else if (_bitStack.CurrentDepth == 0)
			{
				if (_readerOptions.AllowMultipleValues)
				{
					if (!ReadFirstToken(marker))
					{
						return ConsumeTokenResult.NotEnoughDataRollBackState;
					}
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, marker);
			}
			else
			{
				switch (marker)
				{
				case 44:
					_consumed++;
					_bytePositionInLine++;
					if (_consumed >= (uint)_buffer.Length)
					{
						if (IsLastSpan)
						{
							_consumed--;
							_bytePositionInLine--;
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
						}
						return ConsumeTokenResult.NotEnoughDataRollBackState;
					}
					marker = _buffer[_consumed];
					if (marker <= 32)
					{
						SkipWhiteSpace();
						if (!HasMoreData(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
						{
							return ConsumeTokenResult.NotEnoughDataRollBackState;
						}
						marker = _buffer[_consumed];
					}
					if (SkipAllComments(ref marker, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
					{
						TokenStartIndex = _consumed;
						if (_inObject)
						{
							if (marker != 34)
							{
								if (marker == 125)
								{
									if (_readerOptions.AllowTrailingCommas)
									{
										EndObject();
										break;
									}
									ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
								}
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, marker);
							}
							if (!ConsumePropertyName())
							{
								return ConsumeTokenResult.NotEnoughDataRollBackState;
							}
							return ConsumeTokenResult.Success;
						}
						if (marker == 93)
						{
							if (_readerOptions.AllowTrailingCommas)
							{
								EndArray();
								break;
							}
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
						}
						if (!ConsumeValue(marker))
						{
							return ConsumeTokenResult.NotEnoughDataRollBackState;
						}
						return ConsumeTokenResult.Success;
					}
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				case 125:
					EndObject();
					break;
				case 93:
					EndArray();
					break;
				default:
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.FoundInvalidCharacter, marker);
					break;
				}
			}
			return ConsumeTokenResult.Success;
		}
		goto IL_02a0;
		IL_02a0:
		return ConsumeTokenResult.IncompleteNoRollBackNecessary;
	}

	private bool SkipComment()
	{
		ReadOnlySpan<byte> readOnlySpan = _buffer.Slice(_consumed + 1);
		if (readOnlySpan.Length > 0)
		{
			switch (readOnlySpan[0])
			{
			case 47:
			{
				int idx2;
				return SkipSingleLineComment(readOnlySpan.Slice(1), out idx2);
			}
			case 42:
			{
				int idx;
				return SkipMultiLineComment(readOnlySpan.Slice(1), out idx);
			}
			}
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, 47);
		}
		if (IsLastSpan)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, 47);
		}
		return false;
	}

	private bool SkipSingleLineComment(ReadOnlySpan<byte> localBuffer, out int idx)
	{
		idx = FindLineSeparator(localBuffer);
		int num;
		if (idx != -1)
		{
			num = idx;
			if (localBuffer[idx] != 10)
			{
				if (idx < localBuffer.Length - 1)
				{
					if (localBuffer[idx + 1] == 10)
					{
						num++;
					}
				}
				else if (!IsLastSpan)
				{
					return false;
				}
			}
			num++;
			_bytePositionInLine = 0L;
			_lineNumber++;
		}
		else
		{
			if (!IsLastSpan)
			{
				return false;
			}
			idx = localBuffer.Length;
			num = idx;
			_bytePositionInLine += 2 + localBuffer.Length;
		}
		_consumed += 2 + num;
		return true;
	}

	private int FindLineSeparator(ReadOnlySpan<byte> localBuffer)
	{
		int num = 0;
		while (true)
		{
			int num2 = localBuffer.IndexOfAny((byte)10, (byte)13, (byte)226);
			if (num2 == -1)
			{
				return -1;
			}
			num += num2;
			if (localBuffer[num2] != 226)
			{
				break;
			}
			num++;
			localBuffer = localBuffer.Slice(num2 + 1);
			ThrowOnDangerousLineSeparator(localBuffer);
		}
		return num;
	}

	private void ThrowOnDangerousLineSeparator(ReadOnlySpan<byte> localBuffer)
	{
		if (localBuffer.Length >= 2)
		{
			byte b = localBuffer[1];
			if (localBuffer[0] == 128 && (b == 168 || b == 169))
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfLineSeparator, 0);
			}
		}
	}

	private bool SkipMultiLineComment(ReadOnlySpan<byte> localBuffer, out int idx)
	{
		idx = 0;
		while (true)
		{
			int num = localBuffer.Slice(idx).IndexOf((byte)47);
			switch (num)
			{
			case -1:
				if (IsLastSpan)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfCommentNotFound, 0);
				}
				return false;
			default:
				if (localBuffer[num + idx - 1] == 42)
				{
					idx += num - 1;
					_consumed += 4 + idx;
					var (num2, num3) = JsonReaderHelper.CountNewLines(localBuffer.Slice(0, idx));
					_lineNumber += num2;
					if (num3 != -1)
					{
						_bytePositionInLine = idx - num3 + 1;
					}
					else
					{
						_bytePositionInLine += 4 + idx;
					}
					return true;
				}
				break;
			case 0:
				break;
			}
			idx += num + 1;
		}
	}

	private bool ConsumeComment()
	{
		ReadOnlySpan<byte> readOnlySpan = _buffer.Slice(_consumed + 1);
		if (readOnlySpan.Length > 0)
		{
			byte b = readOnlySpan[0];
			switch (b)
			{
			case 47:
				return ConsumeSingleLineComment(readOnlySpan.Slice(1), _consumed);
			case 42:
				return ConsumeMultiLineComment(readOnlySpan.Slice(1), _consumed);
			}
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterAtStartOfComment, b);
		}
		if (IsLastSpan)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
		}
		return false;
	}

	private bool ConsumeSingleLineComment(ReadOnlySpan<byte> localBuffer, int previousConsumed)
	{
		if (!SkipSingleLineComment(localBuffer, out var idx))
		{
			return false;
		}
		ValueSpan = _buffer.Slice(previousConsumed + 2, idx);
		if (_tokenType != JsonTokenType.Comment)
		{
			_previousTokenType = _tokenType;
		}
		_tokenType = JsonTokenType.Comment;
		return true;
	}

	private bool ConsumeMultiLineComment(ReadOnlySpan<byte> localBuffer, int previousConsumed)
	{
		if (!SkipMultiLineComment(localBuffer, out var idx))
		{
			return false;
		}
		ValueSpan = _buffer.Slice(previousConsumed + 2, idx);
		if (_tokenType != JsonTokenType.Comment)
		{
			_previousTokenType = _tokenType;
		}
		_tokenType = JsonTokenType.Comment;
		return true;
	}

	private ReadOnlySpan<byte> GetUnescapedSpan()
	{
		ReadOnlySpan<byte> readOnlySpan = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		if (ValueIsEscaped)
		{
			readOnlySpan = JsonReaderHelper.GetUnescaped(readOnlySpan);
		}
		return readOnlySpan;
	}

	public Utf8JsonReader(ReadOnlySequence<byte> jsonData, bool isFinalBlock, JsonReaderState state)
	{
		ReadOnlyMemory<byte> memory = jsonData.First;
		_buffer = memory.Span;
		_isFinalBlock = isFinalBlock;
		_isInputSequence = true;
		_lineNumber = state._lineNumber;
		_bytePositionInLine = state._bytePositionInLine;
		_inObject = state._inObject;
		_isNotPrimitive = state._isNotPrimitive;
		ValueIsEscaped = state._valueIsEscaped;
		_trailingCommaBeforeComment = state._trailingCommaBeforeComment;
		_tokenType = state._tokenType;
		_previousTokenType = state._previousTokenType;
		_readerOptions = state._readerOptions;
		if (_readerOptions.MaxDepth == 0)
		{
			_readerOptions.MaxDepth = 64;
		}
		_bitStack = state._bitStack;
		_consumed = 0;
		TokenStartIndex = 0L;
		_totalConsumed = 0L;
		ValueSpan = ReadOnlySpan<byte>.Empty;
		_sequence = jsonData;
		HasValueSequence = false;
		ValueSequence = ReadOnlySequence<byte>.Empty;
		if (jsonData.IsSingleSegment)
		{
			_nextPosition = default(SequencePosition);
			_currentPosition = jsonData.Start;
			_isLastSegment = isFinalBlock;
			_isMultiSegment = false;
			return;
		}
		_currentPosition = jsonData.Start;
		_nextPosition = _currentPosition;
		bool flag = _buffer.Length == 0;
		if (flag)
		{
			SequencePosition nextPosition = _nextPosition;
			ReadOnlyMemory<byte> memory2;
			while (jsonData.TryGet(ref _nextPosition, out memory2))
			{
				_currentPosition = nextPosition;
				if (memory2.Length != 0)
				{
					_buffer = memory2.Span;
					break;
				}
				nextPosition = _nextPosition;
			}
		}
		_isLastSegment = !jsonData.TryGet(ref _nextPosition, out memory, !flag) & isFinalBlock;
		_isMultiSegment = true;
	}

	public Utf8JsonReader(ReadOnlySequence<byte> jsonData, JsonReaderOptions options = default(JsonReaderOptions))
		: this(jsonData, isFinalBlock: true, new JsonReaderState(options))
	{
	}

	private bool ReadMultiSegment()
	{
		bool flag = false;
		HasValueSequence = false;
		ValueIsEscaped = false;
		ValueSpan = default(ReadOnlySpan<byte>);
		ValueSequence = default(ReadOnlySequence<byte>);
		if (HasMoreDataMultiSegment())
		{
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpaceMultiSegment();
				if (!HasMoreDataMultiSegment())
				{
					goto IL_0173;
				}
				b = _buffer[_consumed];
			}
			TokenStartIndex = BytesConsumed;
			if (_tokenType != JsonTokenType.None)
			{
				if (b == 47)
				{
					flag = ConsumeNextTokenOrRollbackMultiSegment(b);
				}
				else if (_tokenType == JsonTokenType.StartObject)
				{
					if (b == 125)
					{
						EndObject();
						goto IL_0171;
					}
					if (b != 34)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
					}
					long totalConsumed = _totalConsumed;
					int consumed = _consumed;
					long bytePositionInLine = _bytePositionInLine;
					long lineNumber = _lineNumber;
					SequencePosition currentPosition = _currentPosition;
					flag = ConsumePropertyNameMultiSegment();
					if (!flag)
					{
						_consumed = consumed;
						_tokenType = JsonTokenType.StartObject;
						_bytePositionInLine = bytePositionInLine;
						_lineNumber = lineNumber;
						_totalConsumed = totalConsumed;
						_currentPosition = currentPosition;
					}
				}
				else if (_tokenType != JsonTokenType.StartArray)
				{
					flag = ((_tokenType != JsonTokenType.PropertyName) ? ConsumeNextTokenOrRollbackMultiSegment(b) : ConsumeValueMultiSegment(b));
				}
				else
				{
					if (b == 93)
					{
						EndArray();
						goto IL_0171;
					}
					flag = ConsumeValueMultiSegment(b);
				}
			}
			else
			{
				flag = ReadFirstTokenMultiSegment(b);
			}
		}
		goto IL_0173;
		IL_0173:
		return flag;
		IL_0171:
		flag = true;
		goto IL_0173;
	}

	private bool ValidateStateAtEndOfData()
	{
		if (_bitStack.CurrentDepth != 0)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ZeroDepthAtEnd, 0);
		}
		if (_readerOptions.CommentHandling == JsonCommentHandling.Allow && _tokenType == JsonTokenType.Comment)
		{
			return false;
		}
		if (_tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJsonNonPrimitive, 0);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasMoreDataMultiSegment()
	{
		if (_consumed >= (uint)_buffer.Length)
		{
			if (_isNotPrimitive && IsLastSpan && !ValidateStateAtEndOfData())
			{
				return false;
			}
			if (!GetNextSpan())
			{
				if (_isNotPrimitive && IsLastSpan)
				{
					ValidateStateAtEndOfData();
				}
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasMoreDataMultiSegment(ExceptionResource resource)
	{
		if (_consumed >= (uint)_buffer.Length)
		{
			if (IsLastSpan)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, resource, 0);
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, resource, 0);
				}
				return false;
			}
		}
		return true;
	}

	private bool GetNextSpan()
	{
		ReadOnlyMemory<byte> memory;
		while (true)
		{
			SequencePosition currentPosition = _currentPosition;
			_currentPosition = _nextPosition;
			if (!_sequence.TryGet(ref _nextPosition, out memory))
			{
				_currentPosition = currentPosition;
				_isLastSegment = true;
				return false;
			}
			if (memory.Length != 0)
			{
				break;
			}
			_currentPosition = currentPosition;
		}
		if (_isFinalBlock)
		{
			_isLastSegment = !_sequence.TryGet(ref _nextPosition, out var _, advance: false);
		}
		_buffer = memory.Span;
		_totalConsumed += _consumed;
		_consumed = 0;
		return true;
	}

	private bool ReadFirstTokenMultiSegment(byte first)
	{
		switch (first)
		{
		case 123:
			_bitStack.SetFirstBit();
			_tokenType = JsonTokenType.StartObject;
			ValueSpan = _buffer.Slice(_consumed, 1);
			_consumed++;
			_bytePositionInLine++;
			_inObject = true;
			_isNotPrimitive = true;
			break;
		case 91:
			_bitStack.ResetFirstBit();
			_tokenType = JsonTokenType.StartArray;
			ValueSpan = _buffer.Slice(_consumed, 1);
			_consumed++;
			_bytePositionInLine++;
			_isNotPrimitive = true;
			break;
		default:
		{
			if (JsonHelpers.IsDigit(first) || first == 45)
			{
				if (!TryGetNumberMultiSegment(_buffer.Slice(_consumed), out var consumed))
				{
					return false;
				}
				_tokenType = JsonTokenType.Number;
				_consumed += consumed;
			}
			else if (!ConsumeValueMultiSegment(first))
			{
				return false;
			}
			JsonTokenType tokenType = _tokenType;
			bool isNotPrimitive = ((tokenType == JsonTokenType.StartObject || tokenType == JsonTokenType.StartArray) ? true : false);
			_isNotPrimitive = isNotPrimitive;
			break;
		}
		}
		return true;
	}

	private void SkipWhiteSpaceMultiSegment()
	{
		do
		{
			SkipWhiteSpace();
		}
		while (_consumed >= _buffer.Length && GetNextSpan());
	}

	private bool ConsumeValueMultiSegment(byte marker)
	{
		while (true)
		{
			_trailingCommaBeforeComment = false;
			switch (marker)
			{
			case 34:
				return ConsumeStringMultiSegment();
			case 123:
				StartObject();
				break;
			case 91:
				StartArray();
				break;
			default:
				if (JsonHelpers.IsDigit(marker) || marker == 45)
				{
					return ConsumeNumberMultiSegment();
				}
				switch (marker)
				{
				case 102:
					return ConsumeLiteralMultiSegment(JsonConstants.FalseValue, JsonTokenType.False);
				case 116:
					return ConsumeLiteralMultiSegment(JsonConstants.TrueValue, JsonTokenType.True);
				case 110:
					return ConsumeLiteralMultiSegment(JsonConstants.NullValue, JsonTokenType.Null);
				}
				switch (_readerOptions.CommentHandling)
				{
				case JsonCommentHandling.Allow:
					if (marker == 47)
					{
						SequencePosition currentPosition2 = _currentPosition;
						if (!SkipOrConsumeCommentMultiSegmentWithRollback())
						{
							_currentPosition = currentPosition2;
							return false;
						}
						return true;
					}
					break;
				default:
				{
					if (marker != 47)
					{
						break;
					}
					SequencePosition currentPosition = _currentPosition;
					if (SkipCommentMultiSegment(out var _))
					{
						if (_consumed >= (uint)_buffer.Length)
						{
							if (_isNotPrimitive && IsLastSpan && _tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJsonNonPrimitive, 0);
							}
							if (!GetNextSpan())
							{
								if (_isNotPrimitive && IsLastSpan && _tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
								{
									ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJsonNonPrimitive, 0);
								}
								_currentPosition = currentPosition;
								return false;
							}
						}
						marker = _buffer[_consumed];
						if (marker <= 32)
						{
							SkipWhiteSpaceMultiSegment();
							if (!HasMoreDataMultiSegment())
							{
								_currentPosition = currentPosition;
								return false;
							}
							marker = _buffer[_consumed];
						}
						goto IL_01a8;
					}
					_currentPosition = currentPosition;
					return false;
				}
				case JsonCommentHandling.Disallow:
					break;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, marker);
				break;
			}
			break;
			IL_01a8:
			TokenStartIndex = BytesConsumed;
		}
		return true;
	}

	private bool ConsumeLiteralMultiSegment(ReadOnlySpan<byte> literal, JsonTokenType tokenType)
	{
		ReadOnlySpan<byte> span = _buffer.Slice(_consumed);
		int consumed = literal.Length;
		if (!span.StartsWith(literal))
		{
			int consumed2 = _consumed;
			if (!CheckLiteralMultiSegment(span, literal, out consumed))
			{
				_consumed = consumed2;
				return false;
			}
		}
		else
		{
			ValueSpan = span.Slice(0, literal.Length);
			HasValueSequence = false;
		}
		_tokenType = tokenType;
		_consumed += consumed;
		_bytePositionInLine += consumed;
		return true;
	}

	private bool CheckLiteralMultiSegment(ReadOnlySpan<byte> span, ReadOnlySpan<byte> literal, out int consumed)
	{
		Span<byte> span2 = stackalloc byte[5];
		int num = 0;
		long totalConsumed = _totalConsumed;
		SequencePosition currentPosition = _currentPosition;
		if (span.Length >= literal.Length || IsLastSpan)
		{
			_bytePositionInLine += FindMismatch(span, literal);
			int num2 = AmountToWrite(span, _bytePositionInLine, span2, num);
			span.Slice(0, num2).CopyTo(span2);
			num += num2;
		}
		else if (!literal.StartsWith(span))
		{
			_bytePositionInLine += FindMismatch(span, literal);
			int num3 = AmountToWrite(span, _bytePositionInLine, span2, num);
			span.Slice(0, num3).CopyTo(span2);
			num += num3;
		}
		else
		{
			ReadOnlySpan<byte> readOnlySpan = literal.Slice(span.Length);
			SequencePosition currentPosition2 = _currentPosition;
			int consumed2 = _consumed;
			int num4 = literal.Length - readOnlySpan.Length;
			while (true)
			{
				_totalConsumed += num4;
				_bytePositionInLine += num4;
				if (!GetNextSpan())
				{
					_totalConsumed = totalConsumed;
					consumed = 0;
					_currentPosition = currentPosition;
					if (IsLastSpan)
					{
						break;
					}
					return false;
				}
				int num5 = Math.Min(span.Length, span2.Length - num);
				span.Slice(0, num5).CopyTo(span2.Slice(num));
				num += num5;
				span = _buffer;
				if (span.StartsWith(readOnlySpan))
				{
					HasValueSequence = true;
					SequencePosition start = new SequencePosition(currentPosition2.GetObject(), currentPosition2.GetInteger() + consumed2);
					SequencePosition end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + readOnlySpan.Length);
					ValueSequence = _sequence.Slice(start, end);
					consumed = readOnlySpan.Length;
					return true;
				}
				if (!readOnlySpan.StartsWith(span))
				{
					_bytePositionInLine += FindMismatch(span, readOnlySpan);
					num5 = AmountToWrite(span, _bytePositionInLine, span2, num);
					span.Slice(0, num5).CopyTo(span2.Slice(num));
					num += num5;
					break;
				}
				readOnlySpan = readOnlySpan.Slice(span.Length);
				num4 = span.Length;
			}
		}
		_totalConsumed = totalConsumed;
		consumed = 0;
		_currentPosition = currentPosition;
		throw GetInvalidLiteralMultiSegment(span2.Slice(0, num).ToArray());
		static int AmountToWrite(ReadOnlySpan<byte> readOnlySpan2, long bytePositionInLine, ReadOnlySpan<byte> readSoFar, int written)
		{
			return Math.Min(readSoFar.Length - written, Math.Min(readOnlySpan2.Length, (int)bytePositionInLine + 1));
		}
	}

	private static int FindMismatch(ReadOnlySpan<byte> span, ReadOnlySpan<byte> literal)
	{
		return span.CommonPrefixLength(literal);
	}

	private JsonException GetInvalidLiteralMultiSegment(ReadOnlySpan<byte> span)
	{
		return ThrowHelper.GetJsonReaderException(ref this, span[0] switch
		{
			116 => ExceptionResource.ExpectedTrue, 
			102 => ExceptionResource.ExpectedFalse, 
			_ => ExceptionResource.ExpectedNull, 
		}, 0, span);
	}

	private bool ConsumeNumberMultiSegment()
	{
		if (!TryGetNumberMultiSegment(_buffer.Slice(_consumed), out var consumed))
		{
			return false;
		}
		_tokenType = JsonTokenType.Number;
		_consumed += consumed;
		if (_consumed >= (uint)_buffer.Length && _isNotPrimitive)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, _buffer[_consumed - 1]);
		}
		return true;
	}

	private bool ConsumePropertyNameMultiSegment()
	{
		_trailingCommaBeforeComment = false;
		if (!ConsumeStringMultiSegment())
		{
			return false;
		}
		if (!HasMoreDataMultiSegment(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
		{
			return false;
		}
		byte b = _buffer[_consumed];
		if (b <= 32)
		{
			SkipWhiteSpaceMultiSegment();
			if (!HasMoreDataMultiSegment(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
			{
				return false;
			}
			b = _buffer[_consumed];
		}
		if (b != 58)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedSeparatorAfterPropertyNameNotFound, b);
		}
		_consumed++;
		_bytePositionInLine++;
		_tokenType = JsonTokenType.PropertyName;
		return true;
	}

	private bool ConsumeStringMultiSegment()
	{
		ReadOnlySpan<byte> readOnlySpan = _buffer.Slice(_consumed + 1);
		int num = readOnlySpan.IndexOfQuoteOrAnyControlOrBackSlash();
		if (num >= 0)
		{
			if (readOnlySpan[num] == 34)
			{
				_bytePositionInLine += num + 2;
				ValueSpan = readOnlySpan.Slice(0, num);
				HasValueSequence = false;
				ValueIsEscaped = false;
				_tokenType = JsonTokenType.String;
				_consumed += num + 2;
				return true;
			}
			return ConsumeStringAndValidateMultiSegment(readOnlySpan, num);
		}
		if (IsLastSpan)
		{
			_bytePositionInLine += readOnlySpan.Length + 1;
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
		}
		return ConsumeStringNextSegment();
	}

	private bool ConsumeStringNextSegment()
	{
		PartialStateForRollback state = CaptureState();
		HasValueSequence = true;
		int num = _buffer.Length - _consumed;
		ReadOnlySpan<byte> buffer;
		int num2;
		while (true)
		{
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					_bytePositionInLine += num;
					RollBackState(in state, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
				}
				RollBackState(in state);
				return false;
			}
			buffer = _buffer;
			num2 = buffer.IndexOfQuoteOrAnyControlOrBackSlash();
			if (num2 >= 0)
			{
				break;
			}
			_totalConsumed += buffer.Length;
			_bytePositionInLine += buffer.Length;
		}
		SequencePosition end;
		if (buffer[num2] == 34)
		{
			end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + num2);
			_bytePositionInLine += num + num2 + 1;
			_totalConsumed += num;
			_consumed = num2 + 1;
			ValueIsEscaped = false;
		}
		else
		{
			_bytePositionInLine += num + num2;
			ValueIsEscaped = true;
			bool flag = false;
			while (true)
			{
				if (num2 < buffer.Length)
				{
					byte b = buffer[num2];
					if (b == 34)
					{
						if (!flag)
						{
							break;
						}
						flag = false;
					}
					else if (b == 92)
					{
						flag = !flag;
					}
					else if (flag)
					{
						if (JsonConstants.EscapableChars.IndexOf(b) == -1)
						{
							RollBackState(in state, isError: true);
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterAfterEscapeWithinString, b);
						}
						if (b == 117)
						{
							_bytePositionInLine++;
							int num3 = 0;
							int num4 = num2 + 1;
							while (true)
							{
								if (num4 < buffer.Length)
								{
									byte nextByte = buffer[num4];
									if (!JsonReaderHelper.IsHexDigit(nextByte))
									{
										RollBackState(in state, isError: true);
										ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidHexCharacterWithinString, nextByte);
									}
									num3++;
									_bytePositionInLine++;
									if (num3 >= 4)
									{
										break;
									}
									num4++;
									continue;
								}
								if (!GetNextSpan())
								{
									if (IsLastSpan)
									{
										RollBackState(in state, isError: true);
										ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
									}
									RollBackState(in state);
									return false;
								}
								_totalConsumed += buffer.Length;
								buffer = _buffer;
								num4 = 0;
							}
							flag = false;
							num2 = num4 + 1;
							continue;
						}
						flag = false;
					}
					else if (b < 32)
					{
						RollBackState(in state, isError: true);
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, b);
					}
					_bytePositionInLine++;
					num2++;
					continue;
				}
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						RollBackState(in state, isError: true);
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
					}
					RollBackState(in state);
					return false;
				}
				_totalConsumed += buffer.Length;
				buffer = _buffer;
				num2 = 0;
			}
			_bytePositionInLine++;
			_consumed = num2 + 1;
			_totalConsumed += num;
			end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + num2);
		}
		SequencePosition startPosition = state.GetStartPosition(1);
		ValueSequence = _sequence.Slice(startPosition, end);
		_tokenType = JsonTokenType.String;
		return true;
	}

	private bool ConsumeStringAndValidateMultiSegment(ReadOnlySpan<byte> data, int idx)
	{
		PartialStateForRollback state = CaptureState();
		HasValueSequence = false;
		int num = _buffer.Length - _consumed;
		_bytePositionInLine += idx + 1;
		bool flag = false;
		while (true)
		{
			if (idx < data.Length)
			{
				byte b = data[idx];
				switch (b)
				{
				case 34:
					if (flag)
					{
						flag = false;
						goto IL_01b3;
					}
					if (HasValueSequence)
					{
						_bytePositionInLine++;
						_consumed = idx + 1;
						_totalConsumed += num;
						SequencePosition end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + idx);
						SequencePosition startPosition = state.GetStartPosition(1);
						ValueSequence = _sequence.Slice(startPosition, end);
					}
					else
					{
						_bytePositionInLine++;
						_consumed += idx + 2;
						ValueSpan = data.Slice(0, idx);
					}
					ValueIsEscaped = true;
					_tokenType = JsonTokenType.String;
					return true;
				case 92:
					flag = !flag;
					goto IL_01b3;
				default:
					{
						if (flag)
						{
							if (JsonConstants.EscapableChars.IndexOf(b) == -1)
							{
								RollBackState(in state, isError: true);
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterAfterEscapeWithinString, b);
							}
							if (b == 117)
							{
								_bytePositionInLine++;
								int num2 = 0;
								int num3 = idx + 1;
								while (true)
								{
									if (num3 < data.Length)
									{
										byte nextByte = data[num3];
										if (!JsonReaderHelper.IsHexDigit(nextByte))
										{
											RollBackState(in state, isError: true);
											ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidHexCharacterWithinString, nextByte);
										}
										num2++;
										_bytePositionInLine++;
										if (num2 >= 4)
										{
											break;
										}
										num3++;
										continue;
									}
									if (!GetNextSpan())
									{
										if (IsLastSpan)
										{
											RollBackState(in state, isError: true);
											ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
										}
										RollBackState(in state);
										return false;
									}
									if (HasValueSequence)
									{
										_totalConsumed += data.Length;
									}
									data = _buffer;
									num3 = 0;
									HasValueSequence = true;
								}
								flag = false;
								idx = num3 + 1;
								break;
							}
							flag = false;
						}
						else if (b < 32)
						{
							RollBackState(in state, isError: true);
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, b);
						}
						goto IL_01b3;
					}
					IL_01b3:
					_bytePositionInLine++;
					idx++;
					break;
				}
			}
			else
			{
				if (!GetNextSpan())
				{
					break;
				}
				if (HasValueSequence)
				{
					_totalConsumed += data.Length;
				}
				data = _buffer;
				idx = 0;
				HasValueSequence = true;
			}
		}
		if (IsLastSpan)
		{
			RollBackState(in state, isError: true);
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound, 0);
		}
		RollBackState(in state);
		return false;
	}

	private void RollBackState(scoped in PartialStateForRollback state, bool isError = false)
	{
		_totalConsumed = state._prevTotalConsumed;
		if (!isError)
		{
			_bytePositionInLine = state._prevBytePositionInLine;
		}
		_consumed = state._prevConsumed;
		_currentPosition = state._prevCurrentPosition;
	}

	private bool TryGetNumberMultiSegment(ReadOnlySpan<byte> data, out int consumed)
	{
		PartialStateForRollback rollBackState = CaptureState();
		consumed = 0;
		int i = 0;
		if (ConsumeNegativeSignMultiSegment(ref data, ref i, in rollBackState) == ConsumeNumberResult.NeedMoreData)
		{
			RollBackState(in rollBackState);
			return false;
		}
		byte b = data[i];
		if (b == 48)
		{
			ConsumeNumberResult consumeNumberResult = ConsumeZeroMultiSegment(ref data, ref i, in rollBackState);
			if (consumeNumberResult == ConsumeNumberResult.NeedMoreData)
			{
				RollBackState(in rollBackState);
				return false;
			}
			if (consumeNumberResult != ConsumeNumberResult.Success)
			{
				b = data[i];
				goto IL_00bd;
			}
		}
		else
		{
			ConsumeNumberResult consumeNumberResult2 = ConsumeIntegerDigitsMultiSegment(ref data, ref i);
			if (consumeNumberResult2 == ConsumeNumberResult.NeedMoreData)
			{
				RollBackState(in rollBackState);
				return false;
			}
			if (consumeNumberResult2 != ConsumeNumberResult.Success)
			{
				b = data[i];
				if (b != 46 && b != 69 && b != 101)
				{
					RollBackState(in rollBackState, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, b);
				}
				goto IL_00bd;
			}
		}
		goto IL_01aa;
		IL_00bd:
		if (b == 46)
		{
			i++;
			_bytePositionInLine++;
			ConsumeNumberResult consumeNumberResult3 = ConsumeDecimalDigitsMultiSegment(ref data, ref i, in rollBackState);
			if (consumeNumberResult3 == ConsumeNumberResult.NeedMoreData)
			{
				RollBackState(in rollBackState);
				return false;
			}
			if (consumeNumberResult3 == ConsumeNumberResult.Success)
			{
				goto IL_01aa;
			}
			b = data[i];
			if (b != 69 && b != 101)
			{
				RollBackState(in rollBackState, isError: true);
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedNextDigitEValueNotFound, b);
			}
		}
		i++;
		_bytePositionInLine++;
		if (ConsumeSignMultiSegment(ref data, ref i, in rollBackState) == ConsumeNumberResult.NeedMoreData)
		{
			RollBackState(in rollBackState);
			return false;
		}
		i++;
		_bytePositionInLine++;
		switch (ConsumeIntegerDigitsMultiSegment(ref data, ref i))
		{
		case ConsumeNumberResult.NeedMoreData:
			RollBackState(in rollBackState);
			return false;
		default:
			RollBackState(in rollBackState, isError: true);
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, data[i]);
			break;
		case ConsumeNumberResult.Success:
			break;
		}
		goto IL_01aa;
		IL_01aa:
		if (HasValueSequence)
		{
			SequencePosition startPosition = rollBackState.GetStartPosition();
			SequencePosition end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + i);
			ValueSequence = _sequence.Slice(startPosition, end);
			consumed = i;
		}
		else
		{
			ValueSpan = data.Slice(0, i);
			consumed = i;
		}
		return true;
	}

	private ConsumeNumberResult ConsumeNegativeSignMultiSegment(ref ReadOnlySpan<byte> data, scoped ref int i, scoped in PartialStateForRollback rollBackState)
	{
		byte b = data[i];
		if (b == 45)
		{
			i++;
			_bytePositionInLine++;
			if (i >= data.Length)
			{
				if (IsLastSpan)
				{
					RollBackState(in rollBackState, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						RollBackState(in rollBackState, isError: true);
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
					}
					return ConsumeNumberResult.NeedMoreData;
				}
				_totalConsumed += i;
				HasValueSequence = true;
				i = 0;
				data = _buffer;
			}
			b = data[i];
			if (!JsonHelpers.IsDigit(b))
			{
				RollBackState(in rollBackState, isError: true);
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterSign, b);
			}
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private ConsumeNumberResult ConsumeZeroMultiSegment(ref ReadOnlySpan<byte> data, scoped ref int i, scoped in PartialStateForRollback rollBackState)
	{
		i++;
		_bytePositionInLine++;
		byte value;
		if (i < data.Length)
		{
			value = data[i];
			if (JsonConstants.Delimiters.IndexOf(value) >= 0)
			{
				return ConsumeNumberResult.Success;
			}
		}
		else
		{
			if (IsLastSpan)
			{
				return ConsumeNumberResult.Success;
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					return ConsumeNumberResult.Success;
				}
				return ConsumeNumberResult.NeedMoreData;
			}
			_totalConsumed += i;
			HasValueSequence = true;
			i = 0;
			data = _buffer;
			value = data[i];
			if (JsonConstants.Delimiters.IndexOf(value) >= 0)
			{
				return ConsumeNumberResult.Success;
			}
		}
		value = data[i];
		if (value != 46 && value != 69 && value != 101)
		{
			RollBackState(in rollBackState, isError: true);
			ThrowHelper.ThrowJsonReaderException(ref this, JsonHelpers.IsInRangeInclusive(value, 48, 57) ? ExceptionResource.InvalidLeadingZeroInNumber : ExceptionResource.ExpectedEndOfDigitNotFound, value);
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private ConsumeNumberResult ConsumeIntegerDigitsMultiSegment(ref ReadOnlySpan<byte> data, scoped ref int i)
	{
		byte value = 0;
		int num = 0;
		while (i < data.Length)
		{
			value = data[i];
			if (!JsonHelpers.IsDigit(value))
			{
				break;
			}
			num++;
			i++;
		}
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				_bytePositionInLine += num;
				return ConsumeNumberResult.Success;
			}
			while (true)
			{
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						_bytePositionInLine += num;
						return ConsumeNumberResult.Success;
					}
					return ConsumeNumberResult.NeedMoreData;
				}
				_totalConsumed += i;
				_bytePositionInLine += num;
				num = 0;
				HasValueSequence = true;
				i = 0;
				data = _buffer;
				while (i < data.Length)
				{
					value = data[i];
					if (!JsonHelpers.IsDigit(value))
					{
						break;
					}
					i++;
				}
				_bytePositionInLine += i;
				if (i < data.Length)
				{
					break;
				}
				if (IsLastSpan)
				{
					return ConsumeNumberResult.Success;
				}
			}
		}
		else
		{
			_bytePositionInLine += num;
		}
		if (JsonConstants.Delimiters.IndexOf(value) >= 0)
		{
			return ConsumeNumberResult.Success;
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private ConsumeNumberResult ConsumeDecimalDigitsMultiSegment(ref ReadOnlySpan<byte> data, scoped ref int i, scoped in PartialStateForRollback rollBackState)
	{
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				RollBackState(in rollBackState, isError: true);
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					RollBackState(in rollBackState, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				return ConsumeNumberResult.NeedMoreData;
			}
			_totalConsumed += i;
			HasValueSequence = true;
			i = 0;
			data = _buffer;
		}
		byte b = data[i];
		if (!JsonHelpers.IsDigit(b))
		{
			RollBackState(in rollBackState, isError: true);
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterDecimal, b);
		}
		i++;
		_bytePositionInLine++;
		return ConsumeIntegerDigitsMultiSegment(ref data, ref i);
	}

	private ConsumeNumberResult ConsumeSignMultiSegment(ref ReadOnlySpan<byte> data, scoped ref int i, scoped in PartialStateForRollback rollBackState)
	{
		if (i >= data.Length)
		{
			if (IsLastSpan)
			{
				RollBackState(in rollBackState, isError: true);
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					RollBackState(in rollBackState, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				return ConsumeNumberResult.NeedMoreData;
			}
			_totalConsumed += i;
			HasValueSequence = true;
			i = 0;
			data = _buffer;
		}
		byte b = data[i];
		if (b == 43 || b == 45)
		{
			i++;
			_bytePositionInLine++;
			if (i >= data.Length)
			{
				if (IsLastSpan)
				{
					RollBackState(in rollBackState, isError: true);
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
				}
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						RollBackState(in rollBackState, isError: true);
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundEndOfData, 0);
					}
					return ConsumeNumberResult.NeedMoreData;
				}
				_totalConsumed += i;
				HasValueSequence = true;
				i = 0;
				data = _buffer;
			}
			b = data[i];
		}
		if (!JsonHelpers.IsDigit(b))
		{
			RollBackState(in rollBackState, isError: true);
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.RequiredDigitNotFoundAfterSign, b);
		}
		return ConsumeNumberResult.OperationIncomplete;
	}

	private bool ConsumeNextTokenOrRollbackMultiSegment(byte marker)
	{
		long totalConsumed = _totalConsumed;
		int consumed = _consumed;
		long bytePositionInLine = _bytePositionInLine;
		long lineNumber = _lineNumber;
		JsonTokenType tokenType = _tokenType;
		SequencePosition currentPosition = _currentPosition;
		bool trailingCommaBeforeComment = _trailingCommaBeforeComment;
		switch (ConsumeNextTokenMultiSegment(marker))
		{
		case ConsumeTokenResult.Success:
			return true;
		case ConsumeTokenResult.NotEnoughDataRollBackState:
			_consumed = consumed;
			_tokenType = tokenType;
			_bytePositionInLine = bytePositionInLine;
			_lineNumber = lineNumber;
			_totalConsumed = totalConsumed;
			_currentPosition = currentPosition;
			_trailingCommaBeforeComment = trailingCommaBeforeComment;
			break;
		}
		return false;
	}

	private ConsumeTokenResult ConsumeNextTokenMultiSegment(byte marker)
	{
		if (_readerOptions.CommentHandling != JsonCommentHandling.Disallow)
		{
			if (_readerOptions.CommentHandling != JsonCommentHandling.Allow)
			{
				return ConsumeNextTokenUntilAfterAllCommentsAreSkippedMultiSegment(marker);
			}
			if (marker == 47)
			{
				if (!SkipOrConsumeCommentMultiSegmentWithRollback())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (_tokenType == JsonTokenType.Comment)
			{
				return ConsumeNextTokenFromLastNonCommentTokenMultiSegment();
			}
		}
		if (_bitStack.CurrentDepth == 0)
		{
			if (_readerOptions.AllowMultipleValues)
			{
				if (!ReadFirstTokenMultiSegment(marker))
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, marker);
		}
		switch (marker)
		{
		case 44:
		{
			_consumed++;
			_bytePositionInLine++;
			if (_consumed >= (uint)_buffer.Length)
			{
				if (IsLastSpan)
				{
					_consumed--;
					_bytePositionInLine--;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
				}
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						_consumed--;
						_bytePositionInLine--;
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
					}
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
			}
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpaceMultiSegment();
				if (!HasMoreDataMultiSegment(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				b = _buffer[_consumed];
			}
			TokenStartIndex = BytesConsumed;
			if (_readerOptions.CommentHandling == JsonCommentHandling.Allow && b == 47)
			{
				_trailingCommaBeforeComment = true;
				if (!SkipOrConsumeCommentMultiSegmentWithRollback())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (_inObject)
			{
				if (b != 34)
				{
					if (b == 125)
					{
						if (_readerOptions.AllowTrailingCommas)
						{
							EndObject();
							return ConsumeTokenResult.Success;
						}
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
					}
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
				}
				if (!ConsumePropertyNameMultiSegment())
				{
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				}
				return ConsumeTokenResult.Success;
			}
			if (b == 93)
			{
				if (_readerOptions.AllowTrailingCommas)
				{
					EndArray();
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
			}
			if (!ConsumeValueMultiSegment(b))
			{
				return ConsumeTokenResult.NotEnoughDataRollBackState;
			}
			return ConsumeTokenResult.Success;
		}
		case 125:
			EndObject();
			break;
		case 93:
			EndArray();
			break;
		default:
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.FoundInvalidCharacter, marker);
			break;
		}
		return ConsumeTokenResult.Success;
	}

	private ConsumeTokenResult ConsumeNextTokenFromLastNonCommentTokenMultiSegment()
	{
		if (JsonReaderHelper.IsTokenTypePrimitive(_previousTokenType))
		{
			_tokenType = (_inObject ? JsonTokenType.StartObject : JsonTokenType.StartArray);
		}
		else
		{
			_tokenType = _previousTokenType;
		}
		if (HasMoreDataMultiSegment())
		{
			byte b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpaceMultiSegment();
				if (!HasMoreDataMultiSegment())
				{
					goto IL_03b4;
				}
				b = _buffer[_consumed];
			}
			if (_bitStack.CurrentDepth == 0 && _tokenType != JsonTokenType.None)
			{
				if (_readerOptions.AllowMultipleValues)
				{
					if (!ReadFirstTokenMultiSegment(b))
					{
						return ConsumeTokenResult.NotEnoughDataRollBackState;
					}
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, b);
			}
			TokenStartIndex = BytesConsumed;
			if (b != 44)
			{
				if (b == 125)
				{
					EndObject();
				}
				else
				{
					if (b != 93)
					{
						if (_tokenType == JsonTokenType.None)
						{
							if (ReadFirstTokenMultiSegment(b))
							{
								goto IL_03b2;
							}
						}
						else if (_tokenType == JsonTokenType.StartObject)
						{
							if (b != 34)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
							}
							long totalConsumed = _totalConsumed;
							int consumed = _consumed;
							long bytePositionInLine = _bytePositionInLine;
							long lineNumber = _lineNumber;
							if (ConsumePropertyNameMultiSegment())
							{
								goto IL_03b2;
							}
							_consumed = consumed;
							_tokenType = JsonTokenType.StartObject;
							_bytePositionInLine = bytePositionInLine;
							_lineNumber = lineNumber;
							_totalConsumed = totalConsumed;
						}
						else if (_tokenType == JsonTokenType.StartArray)
						{
							if (ConsumeValueMultiSegment(b))
							{
								goto IL_03b2;
							}
						}
						else if (_tokenType == JsonTokenType.PropertyName)
						{
							if (ConsumeValueMultiSegment(b))
							{
								goto IL_03b2;
							}
						}
						else if (_inObject)
						{
							if (b != 34)
							{
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
							}
							if (ConsumePropertyNameMultiSegment())
							{
								goto IL_03b2;
							}
						}
						else if (ConsumeValueMultiSegment(b))
						{
							goto IL_03b2;
						}
						goto IL_03b4;
					}
					EndArray();
				}
				goto IL_03b2;
			}
			if ((int)_previousTokenType <= 1 || _previousTokenType == JsonTokenType.StartArray || _trailingCommaBeforeComment)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueAfterComment, b);
			}
			_consumed++;
			_bytePositionInLine++;
			if (_consumed >= (uint)_buffer.Length)
			{
				if (IsLastSpan)
				{
					_consumed--;
					_bytePositionInLine--;
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
				}
				if (!GetNextSpan())
				{
					if (IsLastSpan)
					{
						_consumed--;
						_bytePositionInLine--;
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
					}
					goto IL_03b4;
				}
			}
			b = _buffer[_consumed];
			if (b <= 32)
			{
				SkipWhiteSpaceMultiSegment();
				if (!HasMoreDataMultiSegment(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
				{
					goto IL_03b4;
				}
				b = _buffer[_consumed];
			}
			TokenStartIndex = BytesConsumed;
			if (b == 47)
			{
				_trailingCommaBeforeComment = true;
				if (SkipOrConsumeCommentMultiSegmentWithRollback())
				{
					goto IL_03b2;
				}
			}
			else if (_inObject)
			{
				if (b != 34)
				{
					if (b == 125)
					{
						if (_readerOptions.AllowTrailingCommas)
						{
							EndObject();
							goto IL_03b2;
						}
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
					}
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, b);
				}
				if (ConsumePropertyNameMultiSegment())
				{
					goto IL_03b2;
				}
			}
			else
			{
				if (b == 93)
				{
					if (_readerOptions.AllowTrailingCommas)
					{
						EndArray();
						goto IL_03b2;
					}
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
				}
				if (ConsumeValueMultiSegment(b))
				{
					goto IL_03b2;
				}
			}
		}
		goto IL_03b4;
		IL_03b2:
		return ConsumeTokenResult.Success;
		IL_03b4:
		return ConsumeTokenResult.NotEnoughDataRollBackState;
	}

	private bool SkipAllCommentsMultiSegment(scoped ref byte marker)
	{
		while (true)
		{
			if (marker == 47)
			{
				if (!SkipOrConsumeCommentMultiSegmentWithRollback() || !HasMoreDataMultiSegment())
				{
					break;
				}
				marker = _buffer[_consumed];
				if (marker <= 32)
				{
					SkipWhiteSpaceMultiSegment();
					if (!HasMoreDataMultiSegment())
					{
						break;
					}
					marker = _buffer[_consumed];
				}
				continue;
			}
			return true;
		}
		return false;
	}

	private bool SkipAllCommentsMultiSegment(scoped ref byte marker, ExceptionResource resource)
	{
		while (true)
		{
			if (marker == 47)
			{
				if (!SkipOrConsumeCommentMultiSegmentWithRollback() || !HasMoreDataMultiSegment(resource))
				{
					break;
				}
				marker = _buffer[_consumed];
				if (marker <= 32)
				{
					SkipWhiteSpaceMultiSegment();
					if (!HasMoreDataMultiSegment(resource))
					{
						break;
					}
					marker = _buffer[_consumed];
				}
				continue;
			}
			return true;
		}
		return false;
	}

	private ConsumeTokenResult ConsumeNextTokenUntilAfterAllCommentsAreSkippedMultiSegment(byte marker)
	{
		if (SkipAllCommentsMultiSegment(ref marker))
		{
			TokenStartIndex = BytesConsumed;
			if (_tokenType == JsonTokenType.StartObject)
			{
				if (marker == 125)
				{
					EndObject();
				}
				else
				{
					if (marker != 34)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, marker);
					}
					long totalConsumed = _totalConsumed;
					int consumed = _consumed;
					long bytePositionInLine = _bytePositionInLine;
					long lineNumber = _lineNumber;
					SequencePosition currentPosition = _currentPosition;
					if (!ConsumePropertyNameMultiSegment())
					{
						_consumed = consumed;
						_tokenType = JsonTokenType.StartObject;
						_bytePositionInLine = bytePositionInLine;
						_lineNumber = lineNumber;
						_totalConsumed = totalConsumed;
						_currentPosition = currentPosition;
						goto IL_0301;
					}
				}
			}
			else if (_tokenType == JsonTokenType.StartArray)
			{
				if (marker == 93)
				{
					EndArray();
				}
				else if (!ConsumeValueMultiSegment(marker))
				{
					goto IL_0301;
				}
			}
			else if (_tokenType == JsonTokenType.PropertyName)
			{
				if (!ConsumeValueMultiSegment(marker))
				{
					goto IL_0301;
				}
			}
			else if (_bitStack.CurrentDepth == 0)
			{
				if (_readerOptions.AllowMultipleValues)
				{
					if (!ReadFirstTokenMultiSegment(marker))
					{
						return ConsumeTokenResult.NotEnoughDataRollBackState;
					}
					return ConsumeTokenResult.Success;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, marker);
			}
			else
			{
				switch (marker)
				{
				case 44:
					_consumed++;
					_bytePositionInLine++;
					if (_consumed >= (uint)_buffer.Length)
					{
						if (IsLastSpan)
						{
							_consumed--;
							_bytePositionInLine--;
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
						}
						if (!GetNextSpan())
						{
							if (IsLastSpan)
							{
								_consumed--;
								_bytePositionInLine--;
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound, 0);
							}
							return ConsumeTokenResult.NotEnoughDataRollBackState;
						}
					}
					marker = _buffer[_consumed];
					if (marker <= 32)
					{
						SkipWhiteSpaceMultiSegment();
						if (!HasMoreDataMultiSegment(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
						{
							return ConsumeTokenResult.NotEnoughDataRollBackState;
						}
						marker = _buffer[_consumed];
					}
					if (SkipAllCommentsMultiSegment(ref marker, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
					{
						TokenStartIndex = BytesConsumed;
						if (_inObject)
						{
							if (marker != 34)
							{
								if (marker == 125)
								{
									if (_readerOptions.AllowTrailingCommas)
									{
										EndObject();
										break;
									}
									ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeObjectEnd, 0);
								}
								ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, marker);
							}
							if (!ConsumePropertyNameMultiSegment())
							{
								return ConsumeTokenResult.NotEnoughDataRollBackState;
							}
							return ConsumeTokenResult.Success;
						}
						if (marker == 93)
						{
							if (_readerOptions.AllowTrailingCommas)
							{
								EndArray();
								break;
							}
							ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.TrailingCommaNotAllowedBeforeArrayEnd, 0);
						}
						if (!ConsumeValueMultiSegment(marker))
						{
							return ConsumeTokenResult.NotEnoughDataRollBackState;
						}
						return ConsumeTokenResult.Success;
					}
					return ConsumeTokenResult.NotEnoughDataRollBackState;
				case 125:
					EndObject();
					break;
				case 93:
					EndArray();
					break;
				default:
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.FoundInvalidCharacter, marker);
					break;
				}
			}
			return ConsumeTokenResult.Success;
		}
		goto IL_0301;
		IL_0301:
		return ConsumeTokenResult.IncompleteNoRollBackNecessary;
	}

	private bool SkipOrConsumeCommentMultiSegmentWithRollback()
	{
		long bytesConsumed = BytesConsumed;
		SequencePosition start = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + _consumed);
		bool num = SkipCommentMultiSegment(out var tailBytesToIgnore);
		if (num)
		{
			if (_readerOptions.CommentHandling == JsonCommentHandling.Allow)
			{
				SequencePosition end = new SequencePosition(_currentPosition.GetObject(), _currentPosition.GetInteger() + _consumed);
				ReadOnlySequence<byte> readOnlySequence = _sequence.Slice(start, end);
				readOnlySequence = readOnlySequence.Slice(2L, readOnlySequence.Length - 2 - tailBytesToIgnore);
				HasValueSequence = !readOnlySequence.IsSingleSegment;
				if (HasValueSequence)
				{
					ValueSequence = readOnlySequence;
				}
				else
				{
					ValueSpan = readOnlySequence.First.Span;
				}
				if (_tokenType != JsonTokenType.Comment)
				{
					_previousTokenType = _tokenType;
				}
				_tokenType = JsonTokenType.Comment;
				return num;
			}
		}
		else
		{
			_totalConsumed = bytesConsumed;
			_consumed = 0;
		}
		return num;
	}

	private bool SkipCommentMultiSegment(out int tailBytesToIgnore)
	{
		_consumed++;
		_bytePositionInLine++;
		ReadOnlySpan<byte> localBuffer = _buffer.Slice(_consumed);
		if (localBuffer.Length == 0)
		{
			if (IsLastSpan)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
				}
				tailBytesToIgnore = 0;
				return false;
			}
			localBuffer = _buffer;
		}
		byte b = localBuffer[0];
		if (b != 47 && b != 42)
		{
			ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterAtStartOfComment, b);
		}
		bool flag = b == 42;
		_consumed++;
		_bytePositionInLine++;
		localBuffer = localBuffer.Slice(1);
		if (localBuffer.Length == 0)
		{
			if (IsLastSpan)
			{
				tailBytesToIgnore = 0;
				if (flag)
				{
					ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
				}
				return true;
			}
			if (!GetNextSpan())
			{
				tailBytesToIgnore = 0;
				if (IsLastSpan)
				{
					if (flag)
					{
						ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
					}
					return true;
				}
				return false;
			}
			localBuffer = _buffer;
		}
		if (flag)
		{
			tailBytesToIgnore = 2;
			return SkipMultiLineCommentMultiSegment(localBuffer);
		}
		return SkipSingleLineCommentMultiSegment(localBuffer, out tailBytesToIgnore);
	}

	private bool SkipSingleLineCommentMultiSegment(ReadOnlySpan<byte> localBuffer, out int tailBytesToSkip)
	{
		bool flag = false;
		int dangerousLineSeparatorBytesConsumed = 0;
		tailBytesToSkip = 0;
		while (true)
		{
			if (flag)
			{
				if (localBuffer[0] == 10)
				{
					tailBytesToSkip++;
					_consumed++;
				}
				break;
			}
			int num = FindLineSeparatorMultiSegment(localBuffer, ref dangerousLineSeparatorBytesConsumed);
			if (num != -1)
			{
				tailBytesToSkip++;
				_consumed += num + 1;
				_bytePositionInLine += num + 1;
				if (localBuffer[num] == 10)
				{
					break;
				}
				if (num < localBuffer.Length - 1)
				{
					if (localBuffer[num + 1] == 10)
					{
						tailBytesToSkip++;
						_consumed++;
						_bytePositionInLine++;
					}
					break;
				}
				flag = true;
			}
			else
			{
				_consumed += localBuffer.Length;
				_bytePositionInLine += localBuffer.Length;
			}
			if (IsLastSpan)
			{
				if (flag)
				{
					break;
				}
				return true;
			}
			if (!GetNextSpan())
			{
				if (IsLastSpan)
				{
					if (flag)
					{
						break;
					}
					return true;
				}
				return false;
			}
			localBuffer = _buffer;
		}
		_bytePositionInLine = 0L;
		_lineNumber++;
		return true;
	}

	private int FindLineSeparatorMultiSegment(ReadOnlySpan<byte> localBuffer, scoped ref int dangerousLineSeparatorBytesConsumed)
	{
		if (dangerousLineSeparatorBytesConsumed != 0)
		{
			ThrowOnDangerousLineSeparatorMultiSegment(localBuffer, ref dangerousLineSeparatorBytesConsumed);
			if (dangerousLineSeparatorBytesConsumed != 0)
			{
				return -1;
			}
		}
		int num = 0;
		do
		{
			int num2 = localBuffer.IndexOfAny((byte)10, (byte)13, (byte)226);
			dangerousLineSeparatorBytesConsumed = 0;
			if (num2 == -1)
			{
				return -1;
			}
			if (localBuffer[num2] != 226)
			{
				return num + num2;
			}
			int num3 = num2 + 1;
			localBuffer = localBuffer.Slice(num3);
			num += num3;
			dangerousLineSeparatorBytesConsumed++;
			ThrowOnDangerousLineSeparatorMultiSegment(localBuffer, ref dangerousLineSeparatorBytesConsumed);
		}
		while (dangerousLineSeparatorBytesConsumed == 0);
		return -1;
	}

	private void ThrowOnDangerousLineSeparatorMultiSegment(ReadOnlySpan<byte> localBuffer, scoped ref int dangerousLineSeparatorBytesConsumed)
	{
		if (localBuffer.IsEmpty)
		{
			return;
		}
		if (dangerousLineSeparatorBytesConsumed == 1)
		{
			if (localBuffer[0] != 128)
			{
				dangerousLineSeparatorBytesConsumed = 0;
				return;
			}
			localBuffer = localBuffer.Slice(1);
			dangerousLineSeparatorBytesConsumed++;
			if (localBuffer.IsEmpty)
			{
				return;
			}
		}
		if (dangerousLineSeparatorBytesConsumed == 2)
		{
			byte b = localBuffer[0];
			if (b == 168 || b == 169)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfLineSeparator, 0);
			}
			else
			{
				dangerousLineSeparatorBytesConsumed = 0;
			}
		}
	}

	private bool SkipMultiLineCommentMultiSegment(ReadOnlySpan<byte> localBuffer)
	{
		bool flag = false;
		bool flag2 = false;
		while (true)
		{
			if (flag)
			{
				if (localBuffer[0] == 47)
				{
					_consumed++;
					_bytePositionInLine++;
					return true;
				}
				flag = false;
			}
			if (flag2)
			{
				if (localBuffer[0] == 10)
				{
					_consumed++;
					localBuffer = localBuffer.Slice(1);
				}
				flag2 = false;
			}
			int num = localBuffer.IndexOfAny((byte)42, (byte)10, (byte)13);
			if (num != -1)
			{
				int num2 = num + 1;
				byte b = localBuffer[num];
				localBuffer = localBuffer.Slice(num2);
				_consumed += num2;
				switch (b)
				{
				case 42:
					flag = true;
					_bytePositionInLine += num2;
					break;
				case 10:
					_bytePositionInLine = 0L;
					_lineNumber++;
					break;
				default:
					_bytePositionInLine = 0L;
					_lineNumber++;
					flag2 = true;
					break;
				}
			}
			else
			{
				_consumed += localBuffer.Length;
				_bytePositionInLine += localBuffer.Length;
				localBuffer = ReadOnlySpan<byte>.Empty;
			}
			if (!localBuffer.IsEmpty)
			{
				continue;
			}
			if (IsLastSpan)
			{
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
			}
			if (!GetNextSpan())
			{
				if (!IsLastSpan)
				{
					break;
				}
				ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.UnexpectedEndOfDataWhileReadingComment, 0);
			}
			localBuffer = _buffer;
		}
		return false;
	}

	private PartialStateForRollback CaptureState()
	{
		return new PartialStateForRollback(_totalConsumed, _bytePositionInLine, _consumed, _currentPosition);
	}

	public string? GetString()
	{
		if (TokenType == JsonTokenType.Null)
		{
			return null;
		}
		if (TokenType != JsonTokenType.String && TokenType != JsonTokenType.PropertyName)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(TokenType);
		}
		ReadOnlySpan<byte> readOnlySpan = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		if (ValueIsEscaped)
		{
			return JsonReaderHelper.GetUnescapedString(readOnlySpan);
		}
		return JsonReaderHelper.TranscodeHelper(readOnlySpan);
	}

	public readonly int CopyString(Span<byte> utf8Destination)
	{
		JsonTokenType tokenType = _tokenType;
		if ((tokenType != JsonTokenType.PropertyName && tokenType != JsonTokenType.String) || 1 == 0)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(_tokenType);
		}
		return CopyValue(utf8Destination);
	}

	internal readonly int CopyValue(Span<byte> utf8Destination)
	{
		int bytesWritten;
		if (ValueIsEscaped)
		{
			if (!TryCopyEscapedString(utf8Destination, out bytesWritten))
			{
				utf8Destination.Slice(0, bytesWritten).Clear();
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
		}
		else if (HasValueSequence)
		{
			ReadOnlySequence<byte> source = ValueSequence;
			source.CopyTo(utf8Destination);
			bytesWritten = (int)source.Length;
		}
		else
		{
			ReadOnlySpan<byte> valueSpan = ValueSpan;
			valueSpan.CopyTo(utf8Destination);
			bytesWritten = valueSpan.Length;
		}
		JsonReaderHelper.ValidateUtf8(utf8Destination.Slice(0, bytesWritten));
		return bytesWritten;
	}

	public readonly int CopyString(Span<char> destination)
	{
		JsonTokenType tokenType = _tokenType;
		if ((tokenType != JsonTokenType.PropertyName && tokenType != JsonTokenType.String) || 1 == 0)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(_tokenType);
		}
		return CopyValue(destination);
	}

	internal readonly int CopyValue(Span<char> destination)
	{
		byte[] array = null;
		ReadOnlySpan<byte> utf8Unescaped;
		if (ValueIsEscaped)
		{
			int valueLength = ValueLength;
			Span<byte> span = ((valueLength > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(valueLength))) : stackalloc byte[256]);
			Span<byte> destination2 = span;
			TryCopyEscapedString(destination2, out var bytesWritten);
			utf8Unescaped = destination2.Slice(0, bytesWritten);
		}
		else if (HasValueSequence)
		{
			ReadOnlySequence<byte> source = ValueSequence;
			int valueLength = checked((int)source.Length);
			Span<byte> span = ((valueLength > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(valueLength))) : stackalloc byte[256]);
			Span<byte> destination3 = span;
			source.CopyTo(destination3);
			utf8Unescaped = destination3.Slice(0, valueLength);
		}
		else
		{
			utf8Unescaped = ValueSpan;
		}
		int result = JsonReaderHelper.TranscodeHelper(utf8Unescaped, destination);
		if (array != null)
		{
			new Span<byte>(array, 0, utf8Unescaped.Length).Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	private readonly bool TryCopyEscapedString(Span<byte> destination, out int bytesWritten)
	{
		byte[] array = null;
		ReadOnlySpan<byte> source2;
		if (HasValueSequence)
		{
			ReadOnlySequence<byte> source = ValueSequence;
			int num = checked((int)source.Length);
			Span<byte> span = ((num > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(num))) : stackalloc byte[256]);
			Span<byte> destination2 = span;
			source.CopyTo(destination2);
			source2 = destination2.Slice(0, num);
		}
		else
		{
			source2 = ValueSpan;
		}
		bool result = JsonReaderHelper.TryUnescape(source2, destination, out bytesWritten);
		if (array != null)
		{
			new Span<byte>(array, 0, source2.Length).Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	public string GetComment()
	{
		if (TokenType != JsonTokenType.Comment)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedComment(TokenType);
		}
		return JsonReaderHelper.TranscodeHelper(HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
	}

	public bool GetBoolean()
	{
		switch (TokenType)
		{
		case JsonTokenType.True:
			return true;
		default:
			ThrowHelper.ThrowInvalidOperationException_ExpectedBoolean(TokenType);
			break;
		case JsonTokenType.False:
			break;
		}
		return false;
	}

	public byte[] GetBytesFromBase64()
	{
		if (!TryGetBytesFromBase64(out byte[] value))
		{
			ThrowHelper.ThrowFormatException(DataType.Base64String);
		}
		return value;
	}

	public byte GetByte()
	{
		if (!TryGetByte(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Byte);
		}
		return value;
	}

	internal byte GetByteWithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetByteCore(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.Byte);
		}
		return value;
	}

	[CLSCompliant(false)]
	public sbyte GetSByte()
	{
		if (!TryGetSByte(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.SByte);
		}
		return value;
	}

	internal sbyte GetSByteWithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetSByteCore(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.SByte);
		}
		return value;
	}

	public short GetInt16()
	{
		if (!TryGetInt16(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int16);
		}
		return value;
	}

	internal short GetInt16WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetInt16Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int16);
		}
		return value;
	}

	public int GetInt32()
	{
		if (!TryGetInt32(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int32);
		}
		return value;
	}

	internal int GetInt32WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetInt32Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int32);
		}
		return value;
	}

	public long GetInt64()
	{
		if (!TryGetInt64(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int64);
		}
		return value;
	}

	internal long GetInt64WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetInt64Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.Int64);
		}
		return value;
	}

	[CLSCompliant(false)]
	public ushort GetUInt16()
	{
		if (!TryGetUInt16(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt16);
		}
		return value;
	}

	internal ushort GetUInt16WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetUInt16Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt16);
		}
		return value;
	}

	[CLSCompliant(false)]
	public uint GetUInt32()
	{
		if (!TryGetUInt32(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt32);
		}
		return value;
	}

	internal uint GetUInt32WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetUInt32Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt32);
		}
		return value;
	}

	[CLSCompliant(false)]
	public ulong GetUInt64()
	{
		if (!TryGetUInt64(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt64);
		}
		return value;
	}

	internal ulong GetUInt64WithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetUInt64Core(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.UInt64);
		}
		return value;
	}

	public float GetSingle()
	{
		if (!TryGetSingle(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Single);
		}
		return value;
	}

	internal float GetSingleWithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (JsonReaderHelper.TryGetFloatingPointConstant(unescapedSpan, out float value))
		{
			return value;
		}
		if (!Utf8Parser.TryParse(unescapedSpan, out value, out int bytesConsumed, '\0') || unescapedSpan.Length != bytesConsumed || !JsonHelpers.IsFinite(value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Single);
		}
		return value;
	}

	internal float GetSingleFloatingPointConstant()
	{
		if (!JsonReaderHelper.TryGetFloatingPointConstant(GetUnescapedSpan(), out float value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Single);
		}
		return value;
	}

	public double GetDouble()
	{
		if (!TryGetDouble(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Double);
		}
		return value;
	}

	internal double GetDoubleWithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (JsonReaderHelper.TryGetFloatingPointConstant(unescapedSpan, out double value))
		{
			return value;
		}
		if (!Utf8Parser.TryParse(unescapedSpan, out value, out int bytesConsumed, '\0') || unescapedSpan.Length != bytesConsumed || !JsonHelpers.IsFinite(value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Double);
		}
		return value;
	}

	internal double GetDoubleFloatingPointConstant()
	{
		if (!JsonReaderHelper.TryGetFloatingPointConstant(GetUnescapedSpan(), out double value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Double);
		}
		return value;
	}

	public decimal GetDecimal()
	{
		if (!TryGetDecimal(out var value))
		{
			ThrowHelper.ThrowFormatException(NumericType.Decimal);
		}
		return value;
	}

	internal decimal GetDecimalWithQuotes()
	{
		ReadOnlySpan<byte> unescapedSpan = GetUnescapedSpan();
		if (!TryGetDecimalCore(out var value, unescapedSpan))
		{
			ThrowHelper.ThrowFormatException(NumericType.Decimal);
		}
		return value;
	}

	public DateTime GetDateTime()
	{
		if (!TryGetDateTime(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.DateTime);
		}
		return value;
	}

	internal DateTime GetDateTimeNoValidation()
	{
		if (!TryGetDateTimeCore(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.DateTime);
		}
		return value;
	}

	public DateTimeOffset GetDateTimeOffset()
	{
		if (!TryGetDateTimeOffset(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.DateTimeOffset);
		}
		return value;
	}

	internal DateTimeOffset GetDateTimeOffsetNoValidation()
	{
		if (!TryGetDateTimeOffsetCore(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.DateTimeOffset);
		}
		return value;
	}

	public Guid GetGuid()
	{
		if (!TryGetGuid(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.Guid);
		}
		return value;
	}

	internal Guid GetGuidNoValidation()
	{
		if (!TryGetGuidCore(out var value))
		{
			ThrowHelper.ThrowFormatException(DataType.Guid);
		}
		return value;
	}

	public bool TryGetBytesFromBase64([NotNullWhen(true)] out byte[]? value)
	{
		if (TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(TokenType);
		}
		ReadOnlySpan<byte> readOnlySpan = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		if (ValueIsEscaped)
		{
			return JsonReaderHelper.TryGetUnescapedBase64Bytes(readOnlySpan, out value);
		}
		return JsonReaderHelper.TryDecodeBase64(readOnlySpan, out value);
	}

	public bool TryGetByte(out byte value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetByteCore(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetByteCore(out byte value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out byte value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	[CLSCompliant(false)]
	public bool TryGetSByte(out sbyte value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetSByteCore(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetSByteCore(out sbyte value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out sbyte value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	public bool TryGetInt16(out short value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetInt16Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetInt16Core(out short value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out short value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	public bool TryGetInt32(out int value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetInt32Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetInt32Core(out int value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out int value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	public bool TryGetInt64(out long value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetInt64Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetInt64Core(out long value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out long value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0L;
		return false;
	}

	[CLSCompliant(false)]
	public bool TryGetUInt16(out ushort value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetUInt16Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetUInt16Core(out ushort value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out ushort value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	[CLSCompliant(false)]
	public bool TryGetUInt32(out uint value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetUInt32Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetUInt32Core(out uint value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out uint value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0u;
		return false;
	}

	[CLSCompliant(false)]
	public bool TryGetUInt64(out ulong value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetUInt64Core(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetUInt64Core(out ulong value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out ulong value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0uL;
		return false;
	}

	public bool TryGetSingle(out float value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> source = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		if (Utf8Parser.TryParse(source, out float value2, out int bytesConsumed, '\0') && source.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0f;
		return false;
	}

	public bool TryGetDouble(out double value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> source = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		if (Utf8Parser.TryParse(source, out double value2, out int bytesConsumed, '\0') && source.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0.0;
		return false;
	}

	public bool TryGetDecimal(out decimal value)
	{
		if (TokenType != JsonTokenType.Number)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedNumber(TokenType);
		}
		ReadOnlySpan<byte> span = (HasValueSequence ? ((ReadOnlySpan<byte>)ValueSequence.ToArray<byte>()) : ValueSpan);
		return TryGetDecimalCore(out value, span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetDecimalCore(out decimal value, ReadOnlySpan<byte> span)
	{
		if (Utf8Parser.TryParse(span, out decimal value2, out int bytesConsumed, '\0') && span.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0m;
		return false;
	}

	public bool TryGetDateTime(out DateTime value)
	{
		if (TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(TokenType);
		}
		return TryGetDateTimeCore(out value);
	}

	internal bool TryGetDateTimeCore(out DateTime value)
	{
		ReadOnlySpan<byte> segment;
		if (HasValueSequence)
		{
			long length = ValueSequence.Length;
			if (!JsonHelpers.IsInRangeInclusive(length, 10L, 252L))
			{
				value = default(DateTime);
				return false;
			}
			Span<byte> destination = stackalloc byte[252];
			ValueSequence.CopyTo(destination);
			segment = destination.Slice(0, (int)length);
		}
		else
		{
			segment = ValueSpan;
		}
		return JsonReaderHelper.TryGetValue(segment, ValueIsEscaped, out value);
	}

	public bool TryGetDateTimeOffset(out DateTimeOffset value)
	{
		if (TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(TokenType);
		}
		return TryGetDateTimeOffsetCore(out value);
	}

	internal bool TryGetDateTimeOffsetCore(out DateTimeOffset value)
	{
		ReadOnlySpan<byte> segment;
		if (HasValueSequence)
		{
			long length = ValueSequence.Length;
			if (!JsonHelpers.IsInRangeInclusive(length, 10L, 252L))
			{
				value = default(DateTimeOffset);
				return false;
			}
			Span<byte> destination = stackalloc byte[252];
			ValueSequence.CopyTo(destination);
			segment = destination.Slice(0, (int)length);
		}
		else
		{
			segment = ValueSpan;
		}
		return JsonReaderHelper.TryGetValue(segment, ValueIsEscaped, out value);
	}

	public bool TryGetGuid(out Guid value)
	{
		if (TokenType != JsonTokenType.String)
		{
			ThrowHelper.ThrowInvalidOperationException_ExpectedString(TokenType);
		}
		return TryGetGuidCore(out value);
	}

	internal bool TryGetGuidCore(out Guid value)
	{
		ReadOnlySpan<byte> segment;
		if (HasValueSequence)
		{
			long length = ValueSequence.Length;
			if (length > 216)
			{
				value = default(Guid);
				return false;
			}
			Span<byte> destination = stackalloc byte[216];
			ValueSequence.CopyTo(destination);
			segment = destination.Slice(0, (int)length);
		}
		else
		{
			segment = ValueSpan;
		}
		return JsonReaderHelper.TryGetValue(segment, ValueIsEscaped, out value);
	}
}
