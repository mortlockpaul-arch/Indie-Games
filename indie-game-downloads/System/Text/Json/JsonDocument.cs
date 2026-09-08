using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json;

public sealed class JsonDocument : IDisposable
{
	private ref struct PropertyNameSet : IDisposable
	{
		[InlineArray(16)]
		private struct InlineRangeArray16
		{
			private (int Start, int Length) _element0;
		}

		private sealed class PropertyNameComparer : IEqualityComparer<ReadOnlyMemory<byte>>
		{
			internal static readonly PropertyNameComparer Instance = new PropertyNameComparer();

			public bool Equals(ReadOnlyMemory<byte> left, ReadOnlyMemory<byte> right)
			{
				if (left.Length == right.Length)
				{
					return left.Span.SequenceEqual(right.Span);
				}
				return false;
			}

			public int GetHashCode(ReadOnlyMemory<byte> name)
			{
				return System.Marvin.ComputeHash32(name.Span, System.Marvin.DefaultSeed);
			}
		}

		private HashSet<ReadOnlyMemory<byte>> _hashSet = null;

		private int _arraySetCount = 0;

		private bool _useArraySet = true;

		private InlineRangeArray16 _arraySet = default(InlineRangeArray16);

		public PropertyNameSet()
		{
		}

		internal void SetCapacity(int capacity)
		{
			if (capacity <= 16)
			{
				_useArraySet = true;
				return;
			}
			_useArraySet = false;
			if (_hashSet == null)
			{
				_hashSet = new HashSet<ReadOnlyMemory<byte>>(capacity, PropertyNameComparer.Instance);
			}
			else
			{
				_hashSet.EnsureCapacity(capacity);
			}
		}

		internal void AddPropertyName(JsonProperty property, JsonDocument document)
		{
			DbRow dbRow = document._parsedData.Get(property.Value.MetadataDbIndex - 12);
			ReadOnlyMemory<byte> utf8Json = document._utf8Json;
			ReadOnlyMemory<byte> item = utf8Json.Slice(dbRow.Location, dbRow.SizeOrLength);
			if (dbRow.HasComplexChildren)
			{
				SwitchToHashSet(utf8Json);
				item = JsonReaderHelper.GetUnescaped(item.Span);
			}
			if (_useArraySet)
			{
				for (int i = 0; i < _arraySetCount; i++)
				{
					(int, int) tuple = _arraySet[i];
					if (utf8Json.Span.Slice(tuple.Item1, tuple.Item2).SequenceEqual(item.Span))
					{
						ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(item.Span);
					}
				}
				_arraySet[_arraySetCount] = (Start: dbRow.Location, Length: dbRow.SizeOrLength);
				_arraySetCount++;
			}
			else if (!_hashSet.Add(item))
			{
				ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(item.Span);
			}
		}

		private void SwitchToHashSet(ReadOnlyMemory<byte> utf8Json)
		{
			if (_useArraySet)
			{
				if (_hashSet == null)
				{
					_hashSet = new HashSet<ReadOnlyMemory<byte>>(16, PropertyNameComparer.Instance);
				}
				for (int i = 0; i < _arraySetCount; i++)
				{
					(int, int) tuple = _arraySet[i];
					ReadOnlyMemory<byte> item = utf8Json.Slice(tuple.Item1, tuple.Item2);
					_hashSet.Add(item);
				}
				_useArraySet = false;
				_arraySetCount = 0;
			}
		}

		internal void Reset()
		{
			_hashSet?.Clear();
			_arraySetCount = 0;
		}

		public readonly void Dispose()
		{
		}
	}

	internal readonly struct DbRow
	{
		private readonly int _location;

		private readonly int _sizeOrLengthUnion;

		private readonly int _numberOfRowsAndTypeUnion;

		internal int Location => _location;

		internal int SizeOrLength => _sizeOrLengthUnion & 0x7FFFFFFF;

		internal bool IsUnknownSize => _sizeOrLengthUnion == -1;

		internal bool HasComplexChildren => _sizeOrLengthUnion < 0;

		internal int NumberOfRows => _numberOfRowsAndTypeUnion & 0xFFFFFFF;

		internal JsonTokenType TokenType => (JsonTokenType)((uint)_numberOfRowsAndTypeUnion >> 28);

		internal bool IsSimpleValue => (int)TokenType >= 5;

		internal DbRow(JsonTokenType jsonTokenType, int location, int sizeOrLength)
		{
			_location = location;
			_sizeOrLengthUnion = sizeOrLength;
			_numberOfRowsAndTypeUnion = (int)((uint)jsonTokenType << 28);
		}
	}

	private struct MetadataDb : IDisposable
	{
		private byte[] _data;

		private bool _convertToAlloc;

		private bool _isLocked;

		internal int Length { get; private set; }

		private MetadataDb(byte[] initialDb, bool isLocked, bool convertToAlloc)
		{
			_data = initialDb;
			_isLocked = isLocked;
			_convertToAlloc = convertToAlloc;
			Length = 0;
		}

		internal MetadataDb(byte[] completeDb)
		{
			_data = completeDb;
			_isLocked = true;
			_convertToAlloc = false;
			Length = completeDb.Length;
		}

		internal static MetadataDb CreateRented(int payloadLength, bool convertToAlloc)
		{
			int num = payloadLength + 12;
			if (num > 1048576 && num <= 4194304)
			{
				num = 1048576;
			}
			return new MetadataDb(ArrayPool<byte>.Shared.Rent(num), isLocked: false, convertToAlloc);
		}

		internal static MetadataDb CreateLocked(int payloadLength)
		{
			return new MetadataDb(new byte[payloadLength + 12], isLocked: true, convertToAlloc: false);
		}

		public void Dispose()
		{
			byte[] array = Interlocked.Exchange(ref _data, null);
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
				Length = 0;
			}
		}

		internal void CompleteAllocations()
		{
			if (_isLocked)
			{
				return;
			}
			if (_convertToAlloc)
			{
				byte[] data = _data;
				_data = _data.AsSpan(0, Length).ToArray();
				_isLocked = true;
				_convertToAlloc = false;
				ArrayPool<byte>.Shared.Return(data);
			}
			else if (Length <= _data.Length / 2)
			{
				byte[] array = ArrayPool<byte>.Shared.Rent(Length);
				byte[] array2 = array;
				if (array.Length < _data.Length)
				{
					Buffer.BlockCopy(_data, 0, array, 0, Length);
					array2 = _data;
					_data = array;
				}
				ArrayPool<byte>.Shared.Return(array2);
			}
		}

		internal void Append(JsonTokenType tokenType, int startLocation, int length)
		{
			if (Length >= _data.Length - 12)
			{
				Enlarge();
			}
			DbRow value = new DbRow(tokenType, startLocation, length);
			MemoryMarshal.Write(_data.AsSpan(Length), in value);
			Length += 12;
		}

		private void Enlarge()
		{
			byte[] data = _data;
			int num = data.Length * 2;
			if ((uint)num > 2147483591u)
			{
				num = 2147483591;
			}
			if (num == data.Length)
			{
				num = int.MaxValue;
			}
			_data = ArrayPool<byte>.Shared.Rent(num);
			Buffer.BlockCopy(data, 0, _data, 0, data.Length);
			ArrayPool<byte>.Shared.Return(data);
		}

		internal void SetLength(int index, int length)
		{
			MemoryMarshal.Write(_data.AsSpan(index + 4), in length);
		}

		internal void SetNumberOfRows(int index, int numberOfRows)
		{
			Span<byte> span = _data.AsSpan(index + 8);
			MemoryMarshal.Write<int>(span, (MemoryMarshal.Read<int>(span) & -268435456) | numberOfRows);
		}

		internal void SetHasComplexChildren(int index)
		{
			Span<byte> span = _data.AsSpan(index + 4);
			MemoryMarshal.Write<int>(span, MemoryMarshal.Read<int>(span) | int.MinValue);
		}

		internal int FindIndexOfFirstUnsetSizeOrLength(JsonTokenType lookupType)
		{
			return FindOpenElement(lookupType);
		}

		private int FindOpenElement(JsonTokenType lookupType)
		{
			Span<byte> span = _data.AsSpan(0, Length);
			for (int num = Length - 12; num >= 0; num -= 12)
			{
				DbRow dbRow = MemoryMarshal.Read<DbRow>(span.Slice(num));
				if (dbRow.IsUnknownSize && dbRow.TokenType == lookupType)
				{
					return num;
				}
			}
			return -1;
		}

		internal DbRow Get(int index)
		{
			return MemoryMarshal.Read<DbRow>(_data.AsSpan(index));
		}

		internal JsonTokenType GetJsonTokenType(int index)
		{
			return (JsonTokenType)(MemoryMarshal.Read<uint>(_data.AsSpan(index + 8)) >> 28);
		}

		internal MetadataDb CopySegment(int startIndex, int endIndex)
		{
			DbRow dbRow = Get(startIndex);
			int num = endIndex - startIndex;
			byte[] array = new byte[num];
			_data.AsSpan(startIndex, num).CopyTo(array);
			Span<int> span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
			int num2 = span[0];
			if (dbRow.TokenType == JsonTokenType.String)
			{
				num2--;
			}
			for (int num3 = (num - 12) / 4; num3 >= 0; num3 -= 3)
			{
				span[num3] -= num2;
			}
			return new MetadataDb(array);
		}
	}

	private readonly struct StackRow
	{
		internal readonly int SizeOrLength;

		internal readonly int NumberOfRows;

		internal StackRow(int sizeOrLength = 0, int numberOfRows = -1)
		{
			SizeOrLength = sizeOrLength;
			NumberOfRows = numberOfRows;
		}
	}

	private struct StackRowStack : IDisposable
	{
		private byte[] _rentedBuffer;

		private int _topOfStack;

		public StackRowStack(int initialSize)
		{
			_rentedBuffer = ArrayPool<byte>.Shared.Rent(initialSize);
			_topOfStack = _rentedBuffer.Length;
		}

		public void Dispose()
		{
			byte[] rentedBuffer = _rentedBuffer;
			_rentedBuffer = null;
			_topOfStack = 0;
			if (rentedBuffer != null)
			{
				ArrayPool<byte>.Shared.Return(rentedBuffer);
			}
		}

		internal void Push(StackRow row)
		{
			if (_topOfStack < 8)
			{
				Enlarge();
			}
			_topOfStack -= 8;
			MemoryMarshal.Write(_rentedBuffer.AsSpan(_topOfStack), in row);
		}

		internal StackRow Pop()
		{
			StackRow result = MemoryMarshal.Read<StackRow>(_rentedBuffer.AsSpan(_topOfStack));
			_topOfStack += 8;
			return result;
		}

		private void Enlarge()
		{
			byte[] rentedBuffer = _rentedBuffer;
			_rentedBuffer = ArrayPool<byte>.Shared.Rent(rentedBuffer.Length * 2);
			Buffer.BlockCopy(rentedBuffer, _topOfStack, _rentedBuffer, _rentedBuffer.Length - rentedBuffer.Length + _topOfStack, rentedBuffer.Length - _topOfStack);
			_topOfStack += _rentedBuffer.Length - rentedBuffer.Length;
			ArrayPool<byte>.Shared.Return(rentedBuffer);
		}
	}

	private ReadOnlyMemory<byte> _utf8Json;

	private MetadataDb _parsedData;

	private byte[] _extraRentedArrayPoolBytes;

	private PooledByteBufferWriter _extraPooledByteBufferWriter;

	private static JsonDocument s_nullLiteral;

	private static JsonDocument s_trueLiteral;

	private static JsonDocument s_falseLiteral;

	internal bool IsDisposable { get; }

	public JsonElement RootElement => new JsonElement(this, 0);

	private JsonDocument(ReadOnlyMemory<byte> utf8Json, MetadataDb parsedData, byte[] extraRentedArrayPoolBytes = null, PooledByteBufferWriter extraPooledByteBufferWriter = null, bool isDisposable = true)
	{
		_utf8Json = utf8Json;
		_parsedData = parsedData;
		_extraRentedArrayPoolBytes = extraRentedArrayPoolBytes;
		_extraPooledByteBufferWriter = extraPooledByteBufferWriter;
		IsDisposable = isDisposable;
	}

	public void Dispose()
	{
		int length = _utf8Json.Length;
		if (length == 0 || !IsDisposable)
		{
			return;
		}
		_parsedData.Dispose();
		_utf8Json = ReadOnlyMemory<byte>.Empty;
		if (_extraRentedArrayPoolBytes != null)
		{
			byte[] array = Interlocked.Exchange(ref _extraRentedArrayPoolBytes, null);
			if (array != null)
			{
				array.AsSpan(0, length).Clear();
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		else if (_extraPooledByteBufferWriter != null)
		{
			Interlocked.Exchange(ref _extraPooledByteBufferWriter, null)?.Dispose();
		}
	}

	public void WriteTo(Utf8JsonWriter writer)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		RootElement.WriteTo(writer);
	}

	internal JsonTokenType GetJsonTokenType(int index)
	{
		CheckNotDisposed();
		return _parsedData.GetJsonTokenType(index);
	}

	internal bool ValueIsEscaped(int index, bool isPropertyName)
	{
		CheckNotDisposed();
		int index2 = (isPropertyName ? (index - 12) : index);
		return _parsedData.Get(index2).HasComplexChildren;
	}

	internal int GetArrayLength(int index)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.StartArray, dbRow.TokenType);
		return dbRow.SizeOrLength;
	}

	internal int GetPropertyCount(int index)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.StartObject, dbRow.TokenType);
		return dbRow.SizeOrLength;
	}

	internal JsonElement GetArrayIndexElement(int currentIndex, int arrayIndex)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(currentIndex);
		CheckExpectedType(JsonTokenType.StartArray, dbRow.TokenType);
		int sizeOrLength = dbRow.SizeOrLength;
		if ((uint)arrayIndex >= (uint)sizeOrLength)
		{
			throw new IndexOutOfRangeException();
		}
		if (!dbRow.HasComplexChildren)
		{
			return new JsonElement(this, currentIndex + (arrayIndex + 1) * 12);
		}
		int num = 0;
		for (int i = currentIndex + 12; i < _parsedData.Length; i += 12)
		{
			if (arrayIndex == num)
			{
				return new JsonElement(this, i);
			}
			dbRow = _parsedData.Get(i);
			if (!dbRow.IsSimpleValue)
			{
				i += 12 * dbRow.NumberOfRows;
			}
			num++;
		}
		throw new IndexOutOfRangeException();
	}

	internal int GetEndIndex(int index, bool includeEndElement)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		if (dbRow.IsSimpleValue)
		{
			return index + 12;
		}
		int num = index + 12 * dbRow.NumberOfRows;
		if (includeEndElement)
		{
			num += 12;
		}
		return num;
	}

	internal ReadOnlyMemory<byte> GetRootRawValue()
	{
		return GetRawValue(0, includeQuotes: true);
	}

	internal ReadOnlyMemory<byte> GetRawValue(int index, bool includeQuotes)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		if (dbRow.IsSimpleValue)
		{
			if (includeQuotes && dbRow.TokenType == JsonTokenType.String)
			{
				return _utf8Json.Slice(dbRow.Location - 1, dbRow.SizeOrLength + 2);
			}
			return _utf8Json.Slice(dbRow.Location, dbRow.SizeOrLength);
		}
		int endIndex = GetEndIndex(index, includeEndElement: false);
		int location = dbRow.Location;
		dbRow = _parsedData.Get(endIndex);
		return _utf8Json.Slice(location, dbRow.Location - location + dbRow.SizeOrLength);
	}

	private ReadOnlyMemory<byte> GetPropertyRawValue(int valueIndex)
	{
		CheckNotDisposed();
		int num = _parsedData.Get(valueIndex - 12).Location - 1;
		DbRow dbRow = _parsedData.Get(valueIndex);
		int num2;
		if (dbRow.IsSimpleValue)
		{
			num2 = dbRow.Location + dbRow.SizeOrLength;
			if (dbRow.TokenType == JsonTokenType.String)
			{
				num2++;
			}
			return _utf8Json.Slice(num, num2 - num);
		}
		int endIndex = GetEndIndex(valueIndex, includeEndElement: false);
		dbRow = _parsedData.Get(endIndex);
		num2 = dbRow.Location + dbRow.SizeOrLength;
		return _utf8Json.Slice(num, num2 - num);
	}

	internal string GetString(int index, JsonTokenType expectedType)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		JsonTokenType tokenType = dbRow.TokenType;
		if (tokenType == JsonTokenType.Null)
		{
			return null;
		}
		CheckExpectedType(expectedType, tokenType);
		ReadOnlySpan<byte> readOnlySpan = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (!dbRow.HasComplexChildren)
		{
			return JsonReaderHelper.TranscodeHelper(readOnlySpan);
		}
		return JsonReaderHelper.GetUnescapedString(readOnlySpan);
	}

	internal bool TextEquals(int index, ReadOnlySpan<char> otherText, bool isPropertyName)
	{
		CheckNotDisposed();
		byte[] array = null;
		int num = checked(otherText.Length * 3);
		Span<byte> span = ((num > 256) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(num))) : stackalloc byte[256]);
		Span<byte> destination = span;
		bool result = JsonWriterHelper.ToUtf8(otherText, destination, out var written) != OperationStatus.InvalidData && TextEquals(index, destination.Slice(0, written), isPropertyName, shouldUnescape: true);
		if (array != null)
		{
			destination.Slice(0, written).Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return result;
	}

	internal bool TextEquals(int index, ReadOnlySpan<byte> otherUtf8Text, bool isPropertyName, bool shouldUnescape)
	{
		CheckNotDisposed();
		int index2 = (isPropertyName ? (index - 12) : index);
		DbRow dbRow = _parsedData.Get(index2);
		CheckExpectedType(isPropertyName ? JsonTokenType.PropertyName : JsonTokenType.String, dbRow.TokenType);
		ReadOnlySpan<byte> span = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (otherUtf8Text.Length > span.Length || (!shouldUnescape && otherUtf8Text.Length != span.Length))
		{
			return false;
		}
		if (dbRow.HasComplexChildren & shouldUnescape)
		{
			if (otherUtf8Text.Length < span.Length / 6)
			{
				return false;
			}
			int num = span.IndexOf((byte)92);
			if (!otherUtf8Text.StartsWith(span.Slice(0, num)))
			{
				return false;
			}
			return JsonReaderHelper.UnescapeAndCompare(span.Slice(num), otherUtf8Text.Slice(num));
		}
		return span.SequenceEqual(otherUtf8Text);
	}

	internal string GetNameOfPropertyValue(int index)
	{
		return GetString(index - 12, JsonTokenType.PropertyName);
	}

	internal ReadOnlySpan<byte> GetPropertyNameRaw(int index)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index - 12);
		return _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
	}

	internal bool TryGetValue(int index, [NotNullWhen(true)] out byte[] value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.String, dbRow.TokenType);
		ReadOnlySpan<byte> readOnlySpan = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (dbRow.HasComplexChildren)
		{
			return JsonReaderHelper.TryGetUnescapedBase64Bytes(readOnlySpan, out value);
		}
		return JsonReaderHelper.TryDecodeBase64(readOnlySpan, out value);
	}

	internal bool TryGetValue(int index, out sbyte value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out sbyte value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	internal bool TryGetValue(int index, out byte value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out byte value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	internal bool TryGetValue(int index, out short value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out short value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	internal bool TryGetValue(int index, out ushort value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out ushort value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	internal bool TryGetValue(int index, out int value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out int value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0;
		return false;
	}

	internal bool TryGetValue(int index, out uint value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out uint value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0u;
		return false;
	}

	internal bool TryGetValue(int index, out long value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out long value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0L;
		return false;
	}

	internal bool TryGetValue(int index, out ulong value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out ulong value2, out int bytesConsumed, '\0') && bytesConsumed == source.Length)
		{
			value = value2;
			return true;
		}
		value = 0uL;
		return false;
	}

	internal bool TryGetValue(int index, out double value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out double value2, out int bytesConsumed, '\0') && source.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0.0;
		return false;
	}

	internal bool TryGetValue(int index, out float value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out float value2, out int bytesConsumed, '\0') && source.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0f;
		return false;
	}

	internal bool TryGetValue(int index, out decimal value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.Number, dbRow.TokenType);
		ReadOnlySpan<byte> source = _utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength);
		if (Utf8Parser.TryParse(source, out decimal value2, out int bytesConsumed, '\0') && source.Length == bytesConsumed)
		{
			value = value2;
			return true;
		}
		value = 0m;
		return false;
	}

	internal bool TryGetValue(int index, out DateTime value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.String, dbRow.TokenType);
		return JsonReaderHelper.TryGetValue(_utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength), dbRow.HasComplexChildren, out value);
	}

	internal bool TryGetValue(int index, out DateTimeOffset value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.String, dbRow.TokenType);
		return JsonReaderHelper.TryGetValue(_utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength), dbRow.HasComplexChildren, out value);
	}

	internal bool TryGetValue(int index, out Guid value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.String, dbRow.TokenType);
		return JsonReaderHelper.TryGetValue(_utf8Json.Span.Slice(dbRow.Location, dbRow.SizeOrLength), dbRow.HasComplexChildren, out value);
	}

	internal string GetRawValueAsString(int index)
	{
		return JsonReaderHelper.TranscodeHelper(GetRawValue(index, includeQuotes: true).Span);
	}

	internal string GetPropertyRawValueAsString(int valueIndex)
	{
		return JsonReaderHelper.TranscodeHelper(GetPropertyRawValue(valueIndex).Span);
	}

	internal JsonElement CloneElement(int index)
	{
		int endIndex = GetEndIndex(index, includeEndElement: true);
		MetadataDb parsedData = _parsedData.CopySegment(index, endIndex);
		return new JsonDocument(GetRawValue(index, includeQuotes: true).ToArray(), parsedData, null, null, isDisposable: false).RootElement;
	}

	internal void WriteElementTo(int index, Utf8JsonWriter writer)
	{
		CheckNotDisposed();
		DbRow row = _parsedData.Get(index);
		switch (row.TokenType)
		{
		case JsonTokenType.StartObject:
			writer.WriteStartObject();
			WriteComplexElement(index, writer);
			break;
		case JsonTokenType.StartArray:
			writer.WriteStartArray();
			WriteComplexElement(index, writer);
			break;
		case JsonTokenType.String:
			WriteString(in row, writer);
			break;
		case JsonTokenType.Number:
			writer.WriteNumberValue(_utf8Json.Slice(row.Location, row.SizeOrLength).Span);
			break;
		case JsonTokenType.True:
			writer.WriteBooleanValue(value: true);
			break;
		case JsonTokenType.False:
			writer.WriteBooleanValue(value: false);
			break;
		case JsonTokenType.Null:
			writer.WriteNullValue();
			break;
		case JsonTokenType.EndObject:
		case JsonTokenType.EndArray:
		case JsonTokenType.PropertyName:
		case JsonTokenType.Comment:
			break;
		}
	}

	private void WriteComplexElement(int index, Utf8JsonWriter writer)
	{
		int endIndex = GetEndIndex(index, includeEndElement: true);
		for (int i = index + 12; i < endIndex; i += 12)
		{
			DbRow row = _parsedData.Get(i);
			switch (row.TokenType)
			{
			case JsonTokenType.String:
				WriteString(in row, writer);
				break;
			case JsonTokenType.Number:
				writer.WriteNumberValue(_utf8Json.Slice(row.Location, row.SizeOrLength).Span);
				break;
			case JsonTokenType.True:
				writer.WriteBooleanValue(value: true);
				break;
			case JsonTokenType.False:
				writer.WriteBooleanValue(value: false);
				break;
			case JsonTokenType.Null:
				writer.WriteNullValue();
				break;
			case JsonTokenType.StartObject:
				writer.WriteStartObject();
				break;
			case JsonTokenType.EndObject:
				writer.WriteEndObject();
				break;
			case JsonTokenType.StartArray:
				writer.WriteStartArray();
				break;
			case JsonTokenType.EndArray:
				writer.WriteEndArray();
				break;
			case JsonTokenType.PropertyName:
				WritePropertyName(in row, writer);
				break;
			}
		}
	}

	private ReadOnlySpan<byte> UnescapeString(in DbRow row, out ArraySegment<byte> rented)
	{
		int location = row.Location;
		int sizeOrLength = row.SizeOrLength;
		ReadOnlySpan<byte> span = _utf8Json.Slice(location, sizeOrLength).Span;
		if (!row.HasComplexChildren)
		{
			rented = default(ArraySegment<byte>);
			return span;
		}
		byte[] array = ArrayPool<byte>.Shared.Rent(sizeOrLength);
		JsonReaderHelper.Unescape(span, array, out var written);
		rented = new ArraySegment<byte>(array, 0, written);
		return rented.AsSpan();
	}

	private static void ClearAndReturn(ArraySegment<byte> rented)
	{
		if (rented.Array != null)
		{
			rented.AsSpan().Clear();
			ArrayPool<byte>.Shared.Return(rented.Array);
		}
	}

	internal void WritePropertyName(int index, Utf8JsonWriter writer)
	{
		CheckNotDisposed();
		WritePropertyName(_parsedData.Get(index - 12), writer);
	}

	private void WritePropertyName(in DbRow row, Utf8JsonWriter writer)
	{
		ArraySegment<byte> rented = default(ArraySegment<byte>);
		try
		{
			writer.WritePropertyName(UnescapeString(in row, out rented));
		}
		finally
		{
			ClearAndReturn(rented);
		}
	}

	private void WriteString(in DbRow row, Utf8JsonWriter writer)
	{
		ArraySegment<byte> rented = default(ArraySegment<byte>);
		try
		{
			writer.WriteStringValue(UnescapeString(in row, out rented));
		}
		finally
		{
			ClearAndReturn(rented);
		}
	}

	private static void Parse(ReadOnlySpan<byte> utf8JsonSpan, JsonReaderOptions readerOptions, ref MetadataDb database, ref StackRowStack stack)
	{
		bool flag = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		Utf8JsonReader utf8JsonReader = new Utf8JsonReader(utf8JsonSpan, isFinalBlock: true, new JsonReaderState(readerOptions));
		while (utf8JsonReader.Read())
		{
			JsonTokenType tokenType = utf8JsonReader.TokenType;
			int num4 = (int)utf8JsonReader.TokenStartIndex;
			switch (tokenType)
			{
			case JsonTokenType.StartObject:
			{
				if (flag)
				{
					num++;
				}
				num3++;
				database.Append(tokenType, num4, -1);
				StackRow row2 = new StackRow(num, num2 + 1);
				stack.Push(row2);
				num = 0;
				num2 = 0;
				break;
			}
			case JsonTokenType.EndObject:
			{
				int index = database.FindIndexOfFirstUnsetSizeOrLength(JsonTokenType.StartObject);
				num3++;
				num2++;
				database.SetLength(index, num);
				int length2 = database.Length;
				database.Append(tokenType, num4, utf8JsonReader.ValueSpan.Length);
				database.SetNumberOfRows(index, num2);
				database.SetNumberOfRows(length2, num2);
				StackRow stackRow2 = stack.Pop();
				num = stackRow2.SizeOrLength;
				num2 += stackRow2.NumberOfRows;
				break;
			}
			case JsonTokenType.StartArray:
			{
				if (flag)
				{
					num++;
				}
				num2++;
				database.Append(tokenType, num4, -1);
				StackRow row = new StackRow(num, num3 + 1);
				stack.Push(row);
				num = 0;
				num3 = 0;
				break;
			}
			case JsonTokenType.EndArray:
			{
				int num5 = database.FindIndexOfFirstUnsetSizeOrLength(JsonTokenType.StartArray);
				num3++;
				num2++;
				database.SetLength(num5, num);
				database.SetNumberOfRows(num5, num3);
				if (num + 1 != num3)
				{
					database.SetHasComplexChildren(num5);
				}
				int length = database.Length;
				database.Append(tokenType, num4, utf8JsonReader.ValueSpan.Length);
				database.SetNumberOfRows(length, num3);
				StackRow stackRow = stack.Pop();
				num = stackRow.SizeOrLength;
				num3 += stackRow.NumberOfRows;
				break;
			}
			case JsonTokenType.PropertyName:
				num3++;
				num2++;
				num++;
				database.Append(tokenType, num4 + 1, utf8JsonReader.ValueSpan.Length);
				if (utf8JsonReader.ValueIsEscaped)
				{
					database.SetHasComplexChildren(database.Length - 12);
				}
				break;
			default:
				num3++;
				num2++;
				if (flag)
				{
					num++;
				}
				if (tokenType == JsonTokenType.String)
				{
					database.Append(tokenType, num4 + 1, utf8JsonReader.ValueSpan.Length);
					if (utf8JsonReader.ValueIsEscaped)
					{
						database.SetHasComplexChildren(database.Length - 12);
					}
				}
				else
				{
					database.Append(tokenType, num4, utf8JsonReader.ValueSpan.Length);
				}
				break;
			}
			flag = utf8JsonReader.IsInArray;
		}
		database.CompleteAllocations();
	}

	private static void ValidateNoDuplicateProperties(JsonDocument document)
	{
		JsonValueKind valueKind = document.RootElement.ValueKind;
		if (valueKind - 1 <= JsonValueKind.Object)
		{
			ValidateDuplicatePropertiesCore(document);
		}
	}

	private static void ValidateDuplicatePropertiesCore(JsonDocument document)
	{
		using PropertyNameSet propertyNameSet = new PropertyNameSet();
		Stack<int> stack = new Stack<int>();
		int? num = null;
		stack.Push(document.RootElement.MetadataDbIndex);
		do
		{
			JsonElement target = new JsonElement(document, stack.Peek());
			switch (target.ValueKind)
			{
			case JsonValueKind.Object:
			{
				JsonElement.ObjectEnumerator objectEnumerator = new JsonElement.ObjectEnumerator(target, num ?? (-1));
				while (true)
				{
					if (objectEnumerator.MoveNext())
					{
						JsonValueKind valueKind = objectEnumerator.Current.Value.ValueKind;
						if (valueKind - 1 <= JsonValueKind.Object)
						{
							stack.Push(objectEnumerator.Current.Value.MetadataDbIndex);
							num = null;
							break;
						}
						continue;
					}
					objectEnumerator.Reset();
					propertyNameSet.SetCapacity(target.GetPropertyCount());
					foreach (JsonProperty item in objectEnumerator)
					{
						propertyNameSet.AddPropertyName(item, document);
					}
					propertyNameSet.Reset();
					num = stack.Pop();
					break;
				}
				break;
			}
			case JsonValueKind.Array:
			{
				JsonElement.ArrayEnumerator arrayEnumerator = new JsonElement.ArrayEnumerator(target, num ?? (-1));
				while (true)
				{
					if (arrayEnumerator.MoveNext())
					{
						JsonValueKind valueKind = arrayEnumerator.Current.ValueKind;
						if (valueKind - 1 <= JsonValueKind.Object)
						{
							stack.Push(arrayEnumerator.Current.MetadataDbIndex);
							num = null;
							break;
						}
						continue;
					}
					num = stack.Pop();
					break;
				}
				break;
			}
			default:
				ThrowHelper.ThrowJsonException();
				break;
			}
		}
		while (stack.Count != 0);
	}

	private void CheckNotDisposed()
	{
		if (_utf8Json.IsEmpty)
		{
			ThrowHelper.ThrowObjectDisposedException_JsonDocument();
		}
	}

	private static void CheckExpectedType(JsonTokenType expected, JsonTokenType actual)
	{
		if (expected != actual)
		{
			ThrowHelper.ThrowJsonElementWrongTypeException(expected, actual);
		}
	}

	private static void CheckSupportedOptions(JsonReaderOptions readerOptions, string paramName)
	{
		if (readerOptions.CommentHandling == JsonCommentHandling.Allow)
		{
			throw new ArgumentException(System.SR.JsonDocumentDoesNotSupportComments, paramName);
		}
	}

	public static JsonDocument Parse(ReadOnlyMemory<byte> utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		return Parse(utf8Json, options.GetReaderOptions(), null, null, options.AllowDuplicateProperties);
	}

	public static JsonDocument Parse(ReadOnlySequence<byte> utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		JsonReaderOptions readerOptions = options.GetReaderOptions();
		if (utf8Json.IsSingleSegment)
		{
			return Parse(utf8Json.First, readerOptions, null, null, options.AllowDuplicateProperties);
		}
		int num = checked((int)utf8Json.Length);
		byte[] array = ArrayPool<byte>.Shared.Rent(num);
		try
		{
			utf8Json.CopyTo(array.AsSpan());
			return Parse(array.AsMemory(0, num), readerOptions, array, null, options.AllowDuplicateProperties);
		}
		catch
		{
			array.AsSpan(0, num).Clear();
			ArrayPool<byte>.Shared.Return(array);
			throw;
		}
	}

	public static JsonDocument Parse(Stream utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		ArraySegment<byte> segment = ReadToEnd(utf8Json);
		try
		{
			return Parse(segment.AsMemory(), options.GetReaderOptions(), segment.Array, null, options.AllowDuplicateProperties);
		}
		catch
		{
			segment.AsSpan().Clear();
			ArrayPool<byte>.Shared.Return(segment.Array);
			throw;
		}
	}

	internal static JsonDocument ParseRented(PooledByteBufferWriter utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		return Parse(utf8Json.WrittenMemory, options.GetReaderOptions(), null, utf8Json, options.AllowDuplicateProperties);
	}

	internal static JsonDocument ParseValue(Stream utf8Json, JsonDocumentOptions options)
	{
		ArraySegment<byte> segment = ReadToEnd(utf8Json);
		byte[] array = new byte[segment.Count];
		Buffer.BlockCopy(segment.Array, 0, array, 0, segment.Count);
		segment.AsSpan().Clear();
		ArrayPool<byte>.Shared.Return(segment.Array);
		return ParseUnrented(array.AsMemory(), options.GetReaderOptions(), JsonTokenType.None, options.AllowDuplicateProperties);
	}

	internal static JsonDocument ParseValue(ReadOnlySpan<byte> utf8Json, JsonDocumentOptions options)
	{
		byte[] array = new byte[utf8Json.Length];
		utf8Json.CopyTo(array);
		return ParseUnrented(array.AsMemory(), options.GetReaderOptions(), JsonTokenType.None, options.AllowDuplicateProperties);
	}

	internal static JsonDocument ParseValue(string json, JsonDocumentOptions options)
	{
		return ParseValue(json.AsSpan(), options);
	}

	public static Task<JsonDocument> ParseAsync(Stream utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(utf8Json, "utf8Json");
		return ParseAsyncCore(utf8Json, options, cancellationToken);
	}

	private static async Task<JsonDocument> ParseAsyncCore(Stream utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArraySegment<byte> segment = await ReadToEndAsync(utf8Json, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			return Parse(segment.AsMemory(), options.GetReaderOptions(), segment.Array, null, options.AllowDuplicateProperties);
		}
		catch
		{
			segment.AsSpan().Clear();
			ArrayPool<byte>.Shared.Return(segment.Array);
			throw;
		}
	}

	internal static async Task<JsonDocument> ParseAsyncCoreUnrented(Stream utf8Json, JsonDocumentOptions options = default(JsonDocumentOptions), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArraySegment<byte> segment = await ReadToEndAsync(utf8Json, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		byte[] array = new byte[segment.Count];
		Buffer.BlockCopy(segment.Array, 0, array, 0, segment.Count);
		segment.AsSpan().Clear();
		ArrayPool<byte>.Shared.Return(segment.Array);
		return ParseUnrented(array.AsMemory(), options.GetReaderOptions(), JsonTokenType.None, options.AllowDuplicateProperties);
	}

	public static JsonDocument Parse([StringSyntax("Json")] ReadOnlyMemory<char> json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		ReadOnlySpan<char> span = json.Span;
		int utf8ByteCount = JsonReaderHelper.GetUtf8ByteCount(span);
		byte[] array = ArrayPool<byte>.Shared.Rent(utf8ByteCount);
		try
		{
			int utf8FromText = JsonReaderHelper.GetUtf8FromText(span, array);
			return Parse(array.AsMemory(0, utf8FromText), options.GetReaderOptions(), array, null, options.AllowDuplicateProperties);
		}
		catch
		{
			array.AsSpan(0, utf8ByteCount).Clear();
			ArrayPool<byte>.Shared.Return(array);
			throw;
		}
	}

	internal static JsonDocument ParseValue(ReadOnlySpan<char> json, JsonDocumentOptions options)
	{
		int utf8ByteCount = JsonReaderHelper.GetUtf8ByteCount(json);
		byte[] array = ArrayPool<byte>.Shared.Rent(utf8ByteCount);
		byte[] array2;
		try
		{
			int utf8FromText = JsonReaderHelper.GetUtf8FromText(json, array);
			array2 = new byte[utf8FromText];
			Buffer.BlockCopy(array, 0, array2, 0, utf8FromText);
		}
		finally
		{
			array.AsSpan(0, utf8ByteCount).Clear();
			ArrayPool<byte>.Shared.Return(array);
		}
		return ParseUnrented(array2.AsMemory(), options.GetReaderOptions(), JsonTokenType.None, options.AllowDuplicateProperties);
	}

	public static JsonDocument Parse([StringSyntax("Json")] string json, JsonDocumentOptions options = default(JsonDocumentOptions))
	{
		ArgumentNullException.ThrowIfNull(json, "json");
		return Parse(json.AsMemory(), options);
	}

	public static bool TryParseValue(ref Utf8JsonReader reader, [NotNullWhen(true)] out JsonDocument? document)
	{
		return TryParseValue(ref reader, out document, shouldThrow: false, useArrayPools: true);
	}

	public static JsonDocument ParseValue(ref Utf8JsonReader reader)
	{
		return ParseValue(ref reader, allowDuplicateProperties: true);
	}

	internal static JsonDocument ParseValue(ref Utf8JsonReader reader, bool allowDuplicateProperties)
	{
		TryParseValue(ref reader, out var document, shouldThrow: true, useArrayPools: true, allowDuplicateProperties);
		return document;
	}

	internal static bool TryParseValue(ref Utf8JsonReader reader, [NotNullWhen(true)] out JsonDocument document, bool shouldThrow, bool useArrayPools, bool allowDuplicateProperties = true)
	{
		JsonReaderState currentState = reader.CurrentState;
		CheckSupportedOptions(currentState.Options, "reader");
		Utf8JsonReader utf8JsonReader = reader;
		ReadOnlySpan<byte> readOnlySpan = default(ReadOnlySpan<byte>);
		ReadOnlySequence<byte> sequence = default(ReadOnlySequence<byte>);
		try
		{
			JsonTokenType tokenType = reader.TokenType;
			if ((tokenType == JsonTokenType.None || tokenType == JsonTokenType.PropertyName) && !reader.Read())
			{
				if (shouldThrow)
				{
					ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedJsonTokens, 0);
				}
				reader = utf8JsonReader;
				document = null;
				return false;
			}
			switch (reader.TokenType)
			{
			case JsonTokenType.StartObject:
			case JsonTokenType.StartArray:
			{
				long tokenStartIndex = reader.TokenStartIndex;
				if (!reader.TrySkip())
				{
					if (shouldThrow)
					{
						ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedJsonTokens, 0);
					}
					reader = utf8JsonReader;
					document = null;
					return false;
				}
				long num2 = reader.BytesConsumed - tokenStartIndex;
				ReadOnlySequence<byte> originalSequence2 = reader.OriginalSequence;
				if (originalSequence2.IsEmpty)
				{
					readOnlySpan = checked(reader.OriginalSpan.Slice((int)tokenStartIndex, (int)num2));
				}
				else
				{
					sequence = originalSequence2.Slice(tokenStartIndex, num2);
				}
				break;
			}
			case JsonTokenType.True:
			case JsonTokenType.False:
			case JsonTokenType.Null:
				if (useArrayPools)
				{
					if (reader.HasValueSequence)
					{
						sequence = reader.ValueSequence;
					}
					else
					{
						readOnlySpan = reader.ValueSpan;
					}
					break;
				}
				document = CreateForLiteral(reader.TokenType);
				return true;
			case JsonTokenType.Number:
				if (reader.HasValueSequence)
				{
					sequence = reader.ValueSequence;
				}
				else
				{
					readOnlySpan = reader.ValueSpan;
				}
				break;
			case JsonTokenType.String:
			{
				ReadOnlySequence<byte> originalSequence = reader.OriginalSequence;
				if (originalSequence.IsEmpty)
				{
					int length = reader.ValueSpan.Length + 2;
					readOnlySpan = reader.OriginalSpan.Slice((int)reader.TokenStartIndex, length);
				}
				else
				{
					long num = 2L;
					sequence = originalSequence.Slice(length: (!reader.HasValueSequence) ? (num + reader.ValueSpan.Length) : (num + reader.ValueSequence.Length), start: reader.TokenStartIndex);
				}
				break;
			}
			default:
				if (shouldThrow)
				{
					byte nextByte = reader.ValueSpan[0];
					ThrowHelper.ThrowJsonReaderException(ref reader, ExceptionResource.ExpectedStartOfValueNotFound, nextByte);
				}
				reader = utf8JsonReader;
				document = null;
				return false;
			}
		}
		catch
		{
			reader = utf8JsonReader;
			throw;
		}
		int num3 = (readOnlySpan.IsEmpty ? checked((int)sequence.Length) : readOnlySpan.Length);
		if (useArrayPools)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(num3);
			Span<byte> destination = array.AsSpan(0, num3);
			try
			{
				if (readOnlySpan.IsEmpty)
				{
					sequence.CopyTo(destination);
				}
				else
				{
					readOnlySpan.CopyTo(destination);
				}
				document = Parse(array.AsMemory(0, num3), currentState.Options, array, null, allowDuplicateProperties);
			}
			catch
			{
				destination.Clear();
				ArrayPool<byte>.Shared.Return(array);
				throw;
			}
		}
		else
		{
			byte[] array2 = ((!readOnlySpan.IsEmpty) ? readOnlySpan.ToArray() : BuffersExtensions.ToArray(in sequence));
			document = ParseUnrented(array2, currentState.Options, reader.TokenType, allowDuplicateProperties);
		}
		return true;
	}

	private static JsonDocument CreateForLiteral(JsonTokenType tokenType)
	{
		switch (tokenType)
		{
		case JsonTokenType.False:
			if (s_falseLiteral == null)
			{
				s_falseLiteral = Create(JsonConstants.FalseValue.ToArray());
			}
			return s_falseLiteral;
		case JsonTokenType.True:
			if (s_trueLiteral == null)
			{
				s_trueLiteral = Create(JsonConstants.TrueValue.ToArray());
			}
			return s_trueLiteral;
		default:
			if (s_nullLiteral == null)
			{
				s_nullLiteral = Create(JsonConstants.NullValue.ToArray());
			}
			return s_nullLiteral;
		}
		JsonDocument Create(byte[] utf8Json)
		{
			MetadataDb parsedData = MetadataDb.CreateLocked(utf8Json.Length);
			parsedData.Append(tokenType, 0, utf8Json.Length);
			return new JsonDocument(utf8Json, parsedData, null, null, isDisposable: false);
		}
	}

	private static JsonDocument Parse(ReadOnlyMemory<byte> utf8Json, JsonReaderOptions readerOptions, byte[] extraRentedArrayPoolBytes = null, PooledByteBufferWriter extraPooledByteBufferWriter = null, bool allowDuplicateProperties = true)
	{
		ReadOnlySpan<byte> span = utf8Json.Span;
		MetadataDb database = MetadataDb.CreateRented(utf8Json.Length, convertToAlloc: false);
		StackRowStack stack = new StackRowStack(512);
		JsonDocument jsonDocument;
		try
		{
			Parse(span, readerOptions, ref database, ref stack);
			jsonDocument = new JsonDocument(utf8Json, database, extraRentedArrayPoolBytes, extraPooledByteBufferWriter);
			if (!allowDuplicateProperties)
			{
				ValidateNoDuplicateProperties(jsonDocument);
			}
		}
		catch
		{
			database.Dispose();
			throw;
		}
		finally
		{
			stack.Dispose();
		}
		return jsonDocument;
	}

	private static JsonDocument ParseUnrented(ReadOnlyMemory<byte> utf8Json, JsonReaderOptions readerOptions, JsonTokenType tokenType = JsonTokenType.None, bool allowDuplicateProperties = true)
	{
		ReadOnlySpan<byte> span = utf8Json.Span;
		MetadataDb database;
		if (tokenType == JsonTokenType.String || tokenType == JsonTokenType.Number)
		{
			database = MetadataDb.CreateLocked(utf8Json.Length);
			StackRowStack stack = default(StackRowStack);
			Parse(span, readerOptions, ref database, ref stack);
		}
		else
		{
			database = MetadataDb.CreateRented(utf8Json.Length, convertToAlloc: true);
			StackRowStack stack2 = new StackRowStack(512);
			try
			{
				Parse(span, readerOptions, ref database, ref stack2);
			}
			finally
			{
				stack2.Dispose();
			}
		}
		JsonDocument jsonDocument = new JsonDocument(utf8Json, database, null, null, isDisposable: false);
		if (!allowDuplicateProperties)
		{
			ValidateNoDuplicateProperties(jsonDocument);
		}
		return jsonDocument;
	}

	private static ArraySegment<byte> ReadToEnd(Stream stream)
	{
		int num = 0;
		byte[] array = null;
		ReadOnlySpan<byte> utf8Bom = JsonConstants.Utf8Bom;
		try
		{
			if (stream.CanSeek)
			{
				long num2 = Math.Max(utf8Bom.Length, stream.Length - stream.Position) + 1;
				array = ArrayPool<byte>.Shared.Rent(checked((int)num2));
			}
			else
			{
				array = ArrayPool<byte>.Shared.Rent(4096);
			}
			int num3;
			do
			{
				num3 = stream.Read(array, num, utf8Bom.Length - num);
				num += num3;
			}
			while (num3 > 0 && num < utf8Bom.Length);
			if (num == utf8Bom.Length && utf8Bom.SequenceEqual(array.AsSpan(0, utf8Bom.Length)))
			{
				num = 0;
			}
			do
			{
				if (array.Length == num)
				{
					byte[] array2 = array;
					array = ArrayPool<byte>.Shared.Rent(checked(array2.Length * 2));
					Buffer.BlockCopy(array2, 0, array, 0, array2.Length);
					ArrayPool<byte>.Shared.Return(array2, clearArray: true);
				}
				num3 = stream.Read(array, num, array.Length - num);
				num += num3;
			}
			while (num3 > 0);
			return new ArraySegment<byte>(array, 0, num);
		}
		catch
		{
			if (array != null)
			{
				array.AsSpan(0, num).Clear();
				ArrayPool<byte>.Shared.Return(array);
			}
			throw;
		}
	}

	private static async ValueTask<ArraySegment<byte>> ReadToEndAsync(Stream stream, CancellationToken cancellationToken)
	{
		int written = 0;
		byte[] rented = null;
		try
		{
			int utf8BomLength = JsonConstants.Utf8Bom.Length;
			if (stream.CanSeek)
			{
				long num = Math.Max(utf8BomLength, stream.Length - stream.Position) + 1;
				rented = ArrayPool<byte>.Shared.Rent(checked((int)num));
			}
			else
			{
				rented = ArrayPool<byte>.Shared.Rent(4096);
			}
			int num2;
			do
			{
				num2 = await stream.ReadAsync(rented.AsMemory(written, utf8BomLength - written), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				written += num2;
			}
			while (num2 > 0 && written < utf8BomLength);
			if (written == utf8BomLength && JsonConstants.Utf8Bom.SequenceEqual(rented.AsSpan(0, utf8BomLength)))
			{
				written = 0;
			}
			do
			{
				if (rented.Length == written)
				{
					byte[] array = rented;
					rented = ArrayPool<byte>.Shared.Rent(array.Length * 2);
					Buffer.BlockCopy(array, 0, rented, 0, array.Length);
					ArrayPool<byte>.Shared.Return(array, clearArray: true);
				}
				num2 = await stream.ReadAsync(rented.AsMemory(written), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				written += num2;
			}
			while (num2 > 0);
			return new ArraySegment<byte>(rented, 0, written);
		}
		catch
		{
			if (rented != null)
			{
				rented.AsSpan(0, written).Clear();
				ArrayPool<byte>.Shared.Return(rented);
			}
			throw;
		}
	}

	internal bool TryGetNamedPropertyValue(int index, ReadOnlySpan<char> propertyName, out JsonElement value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.StartObject, dbRow.TokenType);
		if (dbRow.NumberOfRows == 1)
		{
			value = default(JsonElement);
			return false;
		}
		int maxByteCount = JsonReaderHelper.s_utf8Encoding.GetMaxByteCount(propertyName.Length);
		int startIndex = index + 12;
		int num = checked(dbRow.NumberOfRows * 12 + index);
		if (maxByteCount < 256)
		{
			Span<byte> span = stackalloc byte[256];
			span = span[..JsonReaderHelper.GetUtf8FromText(propertyName, span)];
			return TryGetNamedPropertyValue(startIndex, num, span, out value);
		}
		int length = propertyName.Length;
		int num2;
		for (num2 = num - 12; num2 > index; num2 -= 12)
		{
			int num3 = num2;
			dbRow = _parsedData.Get(num2);
			num2 = ((!dbRow.IsSimpleValue) ? (num2 - 12 * (dbRow.NumberOfRows + 1)) : (num2 - 12));
			if (_parsedData.Get(num2).SizeOrLength >= length)
			{
				byte[] array = ArrayPool<byte>.Shared.Rent(maxByteCount);
				Span<byte> span2 = default(Span<byte>);
				try
				{
					int utf8FromText = JsonReaderHelper.GetUtf8FromText(propertyName, array);
					span2 = array.AsSpan(0, utf8FromText);
					return TryGetNamedPropertyValue(startIndex, num3 + 12, span2, out value);
				}
				finally
				{
					span2.Clear();
					ArrayPool<byte>.Shared.Return(array);
				}
			}
		}
		value = default(JsonElement);
		return false;
	}

	internal bool TryGetNamedPropertyValue(int index, ReadOnlySpan<byte> propertyName, out JsonElement value)
	{
		CheckNotDisposed();
		DbRow dbRow = _parsedData.Get(index);
		CheckExpectedType(JsonTokenType.StartObject, dbRow.TokenType);
		if (dbRow.NumberOfRows == 1)
		{
			value = default(JsonElement);
			return false;
		}
		int endIndex = checked(dbRow.NumberOfRows * 12 + index);
		return TryGetNamedPropertyValue(index + 12, endIndex, propertyName, out value);
	}

	private bool TryGetNamedPropertyValue(int startIndex, int endIndex, ReadOnlySpan<byte> propertyName, out JsonElement value)
	{
		ReadOnlySpan<byte> span = _utf8Json.Span;
		Span<byte> span2 = stackalloc byte[256];
		int num;
		for (num = endIndex - 12; num > startIndex; num -= 12)
		{
			DbRow dbRow = _parsedData.Get(num);
			num = ((!dbRow.IsSimpleValue) ? (num - 12 * (dbRow.NumberOfRows + 1)) : (num - 12));
			dbRow = _parsedData.Get(num);
			ReadOnlySpan<byte> span3 = span.Slice(dbRow.Location, dbRow.SizeOrLength);
			if (dbRow.HasComplexChildren)
			{
				if (span3.Length > propertyName.Length)
				{
					int num2 = span3.IndexOf((byte)92);
					if (propertyName.Length > num2 && span3.Slice(0, num2).SequenceEqual(propertyName.Slice(0, num2)))
					{
						int num3 = span3.Length - num2;
						int written = 0;
						byte[] array = null;
						try
						{
							Span<byte> destination = ((num3 <= span2.Length) ? span2 : ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(num3))));
							JsonReaderHelper.Unescape(span3.Slice(num2), destination, 0, out written);
							if (((ReadOnlySpan<byte>)destination.Slice(0, written)).SequenceEqual(propertyName.Slice(num2)))
							{
								value = new JsonElement(this, num + 12);
								return true;
							}
						}
						finally
						{
							if (array != null)
							{
								array.AsSpan(0, written).Clear();
								ArrayPool<byte>.Shared.Return(array);
							}
						}
					}
				}
			}
			else if (span3.SequenceEqual(propertyName))
			{
				value = new JsonElement(this, num + 12);
				return true;
			}
		}
		value = default(JsonElement);
		return false;
	}
}
