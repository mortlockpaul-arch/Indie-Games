namespace System.Text.Json;

public readonly struct JsonReaderState
{
	internal readonly long _lineNumber;

	internal readonly long _bytePositionInLine;

	internal readonly bool _inObject;

	internal readonly bool _isNotPrimitive;

	internal readonly bool _valueIsEscaped;

	internal readonly bool _trailingCommaBeforeComment;

	internal readonly JsonTokenType _tokenType;

	internal readonly JsonTokenType _previousTokenType;

	internal readonly JsonReaderOptions _readerOptions;

	internal readonly BitStack _bitStack;

	public JsonReaderOptions Options => _readerOptions;

	public JsonReaderState(JsonReaderOptions options = default(JsonReaderOptions))
	{
		_lineNumber = 0L;
		_bytePositionInLine = 0L;
		_inObject = false;
		_isNotPrimitive = false;
		_valueIsEscaped = false;
		_trailingCommaBeforeComment = false;
		_tokenType = JsonTokenType.None;
		_previousTokenType = JsonTokenType.None;
		_readerOptions = options;
		_bitStack = default(BitStack);
	}

	internal JsonReaderState(long lineNumber, long bytePositionInLine, bool inObject, bool isNotPrimitive, bool valueIsEscaped, bool trailingCommaBeforeComment, JsonTokenType tokenType, JsonTokenType previousTokenType, JsonReaderOptions readerOptions, BitStack bitStack)
	{
		_lineNumber = lineNumber;
		_bytePositionInLine = bytePositionInLine;
		_inObject = inObject;
		_isNotPrimitive = isNotPrimitive;
		_valueIsEscaped = valueIsEscaped;
		_trailingCommaBeforeComment = trailingCommaBeforeComment;
		_tokenType = tokenType;
		_previousTokenType = previousTokenType;
		_readerOptions = readerOptions;
		_bitStack = bitStack;
	}
}
