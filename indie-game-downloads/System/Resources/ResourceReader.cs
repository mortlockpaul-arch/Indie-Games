using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Resources;

public sealed class ResourceReader : IResourceReader, IEnumerable, IDisposable
{
	internal sealed class ResourceEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private readonly ResourceReader _reader;

		private bool _currentIsValid;

		private int _currentName;

		private int _dataPosition;

		public object Key
		{
			get
			{
				if (_currentName == int.MinValue)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
				}
				if (!_currentIsValid)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
				}
				if (_reader._resCache == null)
				{
					throw new InvalidOperationException(SR.ResourceReaderIsClosed);
				}
				return _reader.AllocateStringForNameIndex(_currentName, out _dataPosition);
			}
		}

		public object Current => Entry;

		internal int DataPosition => _dataPosition;

		public DictionaryEntry Entry
		{
			get
			{
				if (_currentName == int.MinValue)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
				}
				if (!_currentIsValid)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
				}
				if (_reader._resCache == null)
				{
					throw new InvalidOperationException(SR.ResourceReaderIsClosed);
				}
				string key = _reader.AllocateStringForNameIndex(_currentName, out _dataPosition);
				object obj = null;
				lock (_reader._resCache)
				{
					if (_reader._resCache.TryGetValue(key, out var value))
					{
						obj = value.Value;
					}
				}
				if (obj == null)
				{
					obj = ((_dataPosition != -1) ? _reader.LoadObject(_dataPosition) : _reader.GetValueForNameIndex(_currentName));
				}
				return new DictionaryEntry(key, obj);
			}
		}

		public object Value
		{
			get
			{
				if (_currentName == int.MinValue)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
				}
				if (!_currentIsValid)
				{
					throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
				}
				if (_reader._resCache == null)
				{
					throw new InvalidOperationException(SR.ResourceReaderIsClosed);
				}
				return _reader.GetValueForNameIndex(_currentName);
			}
		}

		internal ResourceEnumerator(ResourceReader reader)
		{
			_currentName = -1;
			_reader = reader;
			_dataPosition = -2;
		}

		public bool MoveNext()
		{
			if (_currentName == _reader._numResources - 1 || _currentName == int.MinValue)
			{
				_currentIsValid = false;
				_currentName = int.MinValue;
				return false;
			}
			_currentIsValid = true;
			_currentName++;
			return true;
		}

		public void Reset()
		{
			if (_reader._resCache == null)
			{
				throw new InvalidOperationException(SR.ResourceReaderIsClosed);
			}
			_currentIsValid = false;
			_currentName = -1;
		}
	}

	private readonly bool _permitDeserialization;

	private object _binaryFormatter;

	private BinaryReader _store;

	internal Dictionary<string, ResourceLocator> _resCache;

	private long _nameSectionOffset;

	private long _dataSectionOffset;

	private int[] _nameHashes;

	private unsafe int* _nameHashesPtr;

	private int[] _namePositions;

	private unsafe int* _namePositionsPtr;

	private Type[] _typeTable;

	private int[] _typeNamePositions;

	private int _numResources;

	private UnmanagedMemoryStream _ums;

	private int _version;

	[FeatureSwitchDefinition("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization")]
	private static bool EnableUnsafeBinaryFormatterSerialization { get; } = AppContext.TryGetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", out var isEnabled) && isEnabled;

	[FeatureSwitchDefinition("System.Resources.ResourceManager.AllowCustomResourceTypes")]
	internal static bool AllowCustomResourceTypes { get; } = !AppContext.TryGetSwitch("System.Resources.ResourceManager.AllowCustomResourceTypes", out var isEnabled2) || isEnabled2;

	internal ResourceReader(Stream stream, Dictionary<string, ResourceLocator> resCache, bool permitDeserialization)
	{
		_resCache = resCache;
		_store = new BinaryReader(stream, Encoding.UTF8);
		_ums = stream as UnmanagedMemoryStream;
		_permitDeserialization = permitDeserialization;
		ReadResources();
	}

	private object DeserializeObject(int typeIndex)
	{
		if (!AllowCustomResourceTypes)
		{
			throw new NotSupportedException(SR.ResourceManager_ReflectionNotAllowed);
		}
		if (!_permitDeserialization)
		{
			throw new NotSupportedException(SR.NotSupported_ResourceObjectSerialization);
		}
		if (!EnableUnsafeBinaryFormatterSerialization)
		{
			throw new NotSupportedException(SR.BinaryFormatter_SerializationDisallowed);
		}
		if (_binaryFormatter == null)
		{
			InitializeBinaryFormatter();
		}
		Type type = FindType(typeIndex);
		object obj = DeserializeLocal(_store.BaseStream);
		if (obj.GetType() != type)
		{
			throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResType_SerBlobMismatch, type.FullName, obj.GetType().FullName));
		}
		return obj;
		[UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "InitializeBinaryFormatter will get trimmed out when AllowCustomResourceTypes is set to false. When set to true, we will already throw a warning for this feature switch, so we suppress this one in order forthe user to only get one error.")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2121:RequiresUnreferencedCode", Justification = "InitializeBinaryFormatter will get trimmed out when AllowCustomResourceTypes is set to false. When set to true, we will already throw a warning for this feature switch, so we suppress this one in order forthe user to only get one error.")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "InitializeBinaryFormatter will get trimmed out when AllowCustomResourceTypes is set to false. When set to true, we will already throw a warning for this feature switch, so we suppress this one in order forthe user to only get one error.")]
		object DeserializeLocal(Stream stream)
		{
			return Deserialize(_binaryFormatter, stream);
		}
		[UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "InitializeBinaryFormatter will get trimmed out when AllowCustomResourceTypes is set to false. When set to true, we will already throw a warning for this feature switch, so we suppress this one in order forthe user to only get one error.")]
		void InitializeBinaryFormatter()
		{
			_binaryFormatter = CreateBinaryFormatter();
		}
	}

	[RequiresUnreferencedCode("BinaryFormatter serialization is not trim compatible because the type of objects being processed cannot be statically discovered.")]
	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Deserialize")]
	private static extern object Deserialize([UnsafeAccessorType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter, System.Runtime.Serialization.Formatters")] object formatter, Stream serializationStream);

	[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
	[return: UnsafeAccessorType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter, System.Runtime.Serialization.Formatters")]
	private static extern object CreateBinaryFormatter();

	private static bool ValidateReaderType(string readerType)
	{
		return ResourceManager.IsDefaultType(readerType, "System.Resources.ResourceReader");
	}

	public void GetResourceData(string resourceName, out string resourceType, out byte[] resourceData)
	{
		ArgumentNullException.ThrowIfNull(resourceName, "resourceName");
		if (_resCache == null)
		{
			throw new InvalidOperationException(SR.ResourceReaderIsClosed);
		}
		int[] array = new int[_numResources];
		int num = FindPosForResource(resourceName);
		if (num == -1)
		{
			throw new ArgumentException(SR.Format(SR.Arg_ResourceNameNotExist, resourceName));
		}
		lock (this)
		{
			for (int i = 0; i < _numResources; i++)
			{
				_store.BaseStream.Position = _nameSectionOffset + GetNamePosition(i);
				int num2 = _store.Read7BitEncodedInt();
				if (num2 < 0)
				{
					throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesNameInvalidOffset, num2));
				}
				_store.BaseStream.Position += num2;
				int num3 = _store.ReadInt32();
				if (num3 < 0 || num3 >= _store.BaseStream.Length - _dataSectionOffset)
				{
					throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, num3));
				}
				array[i] = num3;
			}
			Array.Sort(array);
			int num4 = Array.BinarySearch(array, num);
			int num5 = (int)(((num4 < _numResources - 1) ? (array[num4 + 1] + _dataSectionOffset) : _store.BaseStream.Length) - (num + _dataSectionOffset));
			_store.BaseStream.Position = _dataSectionOffset + num;
			ResourceTypeCode resourceTypeCode = (ResourceTypeCode)_store.Read7BitEncodedInt();
			if (resourceTypeCode < ResourceTypeCode.Null || (int)resourceTypeCode >= 64 + _typeTable.Length)
			{
				throw new BadImageFormatException(SR.BadImageFormat_InvalidType);
			}
			resourceType = TypeNameFromTypeCode(resourceTypeCode);
			num5 -= (int)(_store.BaseStream.Position - (_dataSectionOffset + num));
			byte[] array2 = _store.ReadBytes(num5);
			if (array2.Length != num5)
			{
				throw new FormatException(SR.BadImageFormat_ResourceNameCorrupted);
			}
			resourceData = array2;
		}
	}

	public ResourceReader(string fileName)
	{
		_resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
		_store = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess), Encoding.UTF8);
		try
		{
			ReadResources();
		}
		catch
		{
			_store.Close();
			throw;
		}
	}

	public ResourceReader(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		if (!stream.CanRead)
		{
			throw new ArgumentException(SR.Argument_StreamNotReadable);
		}
		_resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
		_store = new BinaryReader(stream, Encoding.UTF8);
		_ums = stream as UnmanagedMemoryStream;
		ReadResources();
	}

	public void Close()
	{
		Dispose(disposing: true);
	}

	public void Dispose()
	{
		Close();
	}

	private unsafe void Dispose(bool disposing)
	{
		if (_store != null)
		{
			_resCache = null;
			if (disposing)
			{
				BinaryReader store = _store;
				_store = null;
				store?.Close();
			}
			_store = null;
			_namePositions = null;
			_nameHashes = null;
			_ums = null;
			_namePositionsPtr = null;
			_nameHashesPtr = null;
		}
	}

	private unsafe static int ReadUnalignedI4(int* p)
	{
		return BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(p, 4));
	}

	private void SkipString()
	{
		int num = _store.Read7BitEncodedInt();
		if (num < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
		}
		_store.BaseStream.Seek(num, SeekOrigin.Current);
	}

	private unsafe int GetNameHash(int index)
	{
		if (_ums == null)
		{
			return _nameHashes[index];
		}
		return ReadUnalignedI4(_nameHashesPtr + index);
	}

	private unsafe int GetNamePosition(int index)
	{
		int num = ((_ums != null) ? ReadUnalignedI4(_namePositionsPtr + index) : _namePositions[index]);
		if (num < 0 || num > _dataSectionOffset - _nameSectionOffset)
		{
			throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesNameInvalidOffset, num));
		}
		return num;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		if (_resCache == null)
		{
			throw new InvalidOperationException(SR.ResourceReaderIsClosed);
		}
		return new ResourceEnumerator(this);
	}

	internal ResourceEnumerator GetEnumeratorInternal()
	{
		return new ResourceEnumerator(this);
	}

	internal int FindPosForResource(string name)
	{
		int num = FastResourceComparer.HashFunction(name);
		int num2 = 0;
		int i = _numResources - 1;
		int num3 = -1;
		bool flag = false;
		while (num2 <= i)
		{
			num3 = num2 + i >> 1;
			int nameHash = GetNameHash(num3);
			int num4 = ((nameHash != num) ? ((nameHash >= num) ? 1 : (-1)) : 0);
			if (num4 == 0)
			{
				flag = true;
				break;
			}
			if (num4 < 0)
			{
				num2 = num3 + 1;
			}
			else
			{
				i = num3 - 1;
			}
		}
		if (!flag)
		{
			return -1;
		}
		if (num2 != num3)
		{
			num2 = num3;
			while (num2 > 0 && GetNameHash(num2 - 1) == num)
			{
				num2--;
			}
		}
		if (i != num3)
		{
			for (i = num3; i < _numResources - 1 && GetNameHash(i + 1) == num; i++)
			{
			}
		}
		lock (this)
		{
			for (int j = num2; j <= i; j++)
			{
				_store.BaseStream.Seek(_nameSectionOffset + GetNamePosition(j), SeekOrigin.Begin);
				if (CompareStringEqualsName(name))
				{
					int num5 = _store.ReadInt32();
					if (num5 < 0 || num5 >= _store.BaseStream.Length - _dataSectionOffset)
					{
						throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, num5));
					}
					return num5;
				}
			}
		}
		return -1;
	}

	private unsafe bool CompareStringEqualsName(string name)
	{
		int num = _store.Read7BitEncodedInt();
		if (num < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
		}
		if (_ums != null)
		{
			byte* positionPointer = _ums.PositionPointer;
			_ums.Seek(num, SeekOrigin.Current);
			if (_ums.Position > _ums.Length)
			{
				throw new BadImageFormatException(SR.BadImageFormat_ResourcesNameTooLong);
			}
			return FastResourceComparer.CompareOrdinal(positionPointer, num, name) == 0;
		}
		byte[] array = new byte[num];
		int num2 = num;
		while (num2 > 0)
		{
			int num3 = _store.Read(array, num - num2, num2);
			if (num3 == 0)
			{
				throw new BadImageFormatException(SR.BadImageFormat_ResourceNameCorrupted);
			}
			num2 -= num3;
		}
		return FastResourceComparer.CompareOrdinal(array, num / 2, name) == 0;
	}

	private unsafe string AllocateStringForNameIndex(int index, out int dataOffset)
	{
		long num = GetNamePosition(index);
		int num2;
		byte[] array;
		lock (this)
		{
			_store.BaseStream.Seek(num + _nameSectionOffset, SeekOrigin.Begin);
			num2 = _store.Read7BitEncodedInt();
			if (num2 < 0)
			{
				throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
			}
			if (_ums != null)
			{
				if (_ums.Position > _ums.Length - num2)
				{
					throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourcesIndexTooLong, index));
				}
				string text = null;
				char* positionPointer = (char*)_ums.PositionPointer;
				_ = BitConverter.IsLittleEndian;
				text = new string(positionPointer, 0, num2 / 2);
				_ums.Position += num2;
				dataOffset = _store.ReadInt32();
				if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
				{
					throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
				}
				return text;
			}
			array = new byte[num2];
			int num3 = num2;
			while (num3 > 0)
			{
				int num4 = _store.Read(array, num2 - num3, num3);
				if (num4 == 0)
				{
					throw new EndOfStreamException(SR.Format(SR.BadImageFormat_ResourceNameCorrupted_NameIndex, index));
				}
				num3 -= num4;
			}
			dataOffset = _store.ReadInt32();
			if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
			{
				throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
			}
		}
		return Encoding.Unicode.GetString(array, 0, num2);
	}

	private object GetValueForNameIndex(int index)
	{
		long num = GetNamePosition(index);
		lock (this)
		{
			_store.BaseStream.Seek(num + _nameSectionOffset, SeekOrigin.Begin);
			SkipString();
			int num2 = _store.ReadInt32();
			if (num2 < 0 || num2 >= _store.BaseStream.Length - _dataSectionOffset)
			{
				throw new FormatException(SR.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, num2));
			}
			if (_version == 1)
			{
				return LoadObjectV1(num2);
			}
			ResourceTypeCode typeCode;
			return LoadObjectV2(num2, out typeCode);
		}
	}

	internal string LoadString(int pos)
	{
		lock (this)
		{
			_store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
			string result = null;
			int num = _store.Read7BitEncodedInt();
			if (_version == 1)
			{
				if (num == -1)
				{
					return null;
				}
				if (FindType(num) != typeof(string))
				{
					throw new InvalidOperationException(SR.Format(SR.InvalidOperation_ResourceNotString_Type, FindType(num).FullName));
				}
				result = _store.ReadString();
			}
			else
			{
				ResourceTypeCode resourceTypeCode = (ResourceTypeCode)num;
				if (resourceTypeCode != ResourceTypeCode.String && resourceTypeCode != ResourceTypeCode.Null)
				{
					throw new InvalidOperationException(SR.Format(p1: (resourceTypeCode >= ResourceTypeCode.StartOfUserTypes) ? FindType((int)(resourceTypeCode - 64)).FullName : resourceTypeCode.ToString(), resourceFormat: SR.InvalidOperation_ResourceNotString_Type));
				}
				if (resourceTypeCode == ResourceTypeCode.String)
				{
					result = _store.ReadString();
				}
			}
			return result;
		}
	}

	internal object LoadObject(int pos)
	{
		lock (this)
		{
			ResourceTypeCode typeCode;
			return (_version == 1) ? LoadObjectV1(pos) : LoadObjectV2(pos, out typeCode);
		}
	}

	internal object LoadObject(int pos, out ResourceTypeCode typeCode)
	{
		lock (this)
		{
			if (_version == 1)
			{
				object obj = LoadObjectV1(pos);
				typeCode = ((obj is string) ? ResourceTypeCode.String : ResourceTypeCode.StartOfUserTypes);
				return obj;
			}
			return LoadObjectV2(pos, out typeCode);
		}
	}

	private object LoadObjectV1(int pos)
	{
		try
		{
			return _LoadObjectV1(pos);
		}
		catch (EndOfStreamException inner)
		{
			throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner);
		}
		catch (ArgumentOutOfRangeException inner2)
		{
			throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner2);
		}
	}

	private object _LoadObjectV1(int pos)
	{
		_store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
		int num = _store.Read7BitEncodedInt();
		if (num == -1)
		{
			return null;
		}
		Type type = FindType(num);
		if (type == typeof(string))
		{
			return _store.ReadString();
		}
		if (type == typeof(int))
		{
			return _store.ReadInt32();
		}
		if (type == typeof(byte))
		{
			return _store.ReadByte();
		}
		if (type == typeof(sbyte))
		{
			return _store.ReadSByte();
		}
		if (type == typeof(short))
		{
			return _store.ReadInt16();
		}
		if (type == typeof(long))
		{
			return _store.ReadInt64();
		}
		if (type == typeof(ushort))
		{
			return _store.ReadUInt16();
		}
		if (type == typeof(uint))
		{
			return _store.ReadUInt32();
		}
		if (type == typeof(ulong))
		{
			return _store.ReadUInt64();
		}
		if (type == typeof(float))
		{
			return _store.ReadSingle();
		}
		if (type == typeof(double))
		{
			return _store.ReadDouble();
		}
		if (type == typeof(DateTime))
		{
			return new DateTime(_store.ReadInt64());
		}
		if (type == typeof(TimeSpan))
		{
			return new TimeSpan(_store.ReadInt64());
		}
		if (type == typeof(decimal))
		{
			Span<int> span = stackalloc int[4];
			for (int i = 0; i < span.Length; i++)
			{
				span[i] = _store.ReadInt32();
			}
			return new decimal(span);
		}
		return DeserializeObject(num);
	}

	private object LoadObjectV2(int pos, out ResourceTypeCode typeCode)
	{
		try
		{
			return _LoadObjectV2(pos, out typeCode);
		}
		catch (EndOfStreamException inner)
		{
			throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner);
		}
		catch (ArgumentOutOfRangeException inner2)
		{
			throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner2);
		}
	}

	private unsafe object _LoadObjectV2(int pos, out ResourceTypeCode typeCode)
	{
		_store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
		typeCode = (ResourceTypeCode)_store.Read7BitEncodedInt();
		switch (typeCode)
		{
		case ResourceTypeCode.Null:
			return null;
		case ResourceTypeCode.String:
			return _store.ReadString();
		case ResourceTypeCode.Boolean:
			return _store.ReadBoolean();
		case ResourceTypeCode.Char:
			return (char)_store.ReadUInt16();
		case ResourceTypeCode.Byte:
			return _store.ReadByte();
		case ResourceTypeCode.SByte:
			return _store.ReadSByte();
		case ResourceTypeCode.Int16:
			return _store.ReadInt16();
		case ResourceTypeCode.UInt16:
			return _store.ReadUInt16();
		case ResourceTypeCode.Int32:
			return _store.ReadInt32();
		case ResourceTypeCode.UInt32:
			return _store.ReadUInt32();
		case ResourceTypeCode.Int64:
			return _store.ReadInt64();
		case ResourceTypeCode.UInt64:
			return _store.ReadUInt64();
		case ResourceTypeCode.Single:
			return _store.ReadSingle();
		case ResourceTypeCode.Double:
			return _store.ReadDouble();
		case ResourceTypeCode.Decimal:
			return _store.ReadDecimal();
		case ResourceTypeCode.DateTime:
			return DateTime.FromBinary(_store.ReadInt64());
		case ResourceTypeCode.TimeSpan:
			return new TimeSpan(_store.ReadInt64());
		case ResourceTypeCode.ByteArray:
		{
			int num2 = _store.ReadInt32();
			if (num2 < 0)
			{
				throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num2));
			}
			if (_ums == null)
			{
				if (num2 > _store.BaseStream.Length)
				{
					throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num2));
				}
				return _store.ReadBytes(num2);
			}
			if (num2 > _ums.Length - _ums.Position)
			{
				throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num2));
			}
			byte[] array = new byte[num2];
			_ums.Read(array, 0, num2);
			return array;
		}
		case ResourceTypeCode.Stream:
		{
			int num = _store.ReadInt32();
			if (num < 0)
			{
				throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num));
			}
			if (_ums == null)
			{
				return new PinnedBufferMemoryStream(_store.ReadBytes(num));
			}
			if (num > _ums.Length - _ums.Position)
			{
				throw new BadImageFormatException(SR.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num));
			}
			return new UnmanagedMemoryStream(_ums.PositionPointer, num, num, FileAccess.Read);
		}
		default:
		{
			if (typeCode < ResourceTypeCode.StartOfUserTypes)
			{
				throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch);
			}
			int typeIndex = (int)(typeCode - 64);
			return DeserializeObject(typeIndex);
		}
		}
	}

	[MemberNotNull("_typeTable")]
	[MemberNotNull("_typeNamePositions")]
	private void ReadResources()
	{
		try
		{
			_ReadResources();
		}
		catch (EndOfStreamException inner)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted, inner);
		}
		catch (IndexOutOfRangeException inner2)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted, inner2);
		}
	}

	[MemberNotNull("_typeTable")]
	[MemberNotNull("_typeNamePositions")]
	private unsafe void _ReadResources()
	{
		if (_store.ReadInt32() != ResourceManager.MagicNumber)
		{
			throw new ArgumentException(SR.Resources_StreamNotValid);
		}
		int num = _store.ReadInt32();
		int num2 = _store.ReadInt32();
		if (num2 < 0 || num < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
		}
		if (num > 1)
		{
			_store.BaseStream.Seek(num2, SeekOrigin.Current);
		}
		else
		{
			string text = _store.ReadString();
			if (!ValidateReaderType(text))
			{
				throw new NotSupportedException(SR.Format(SR.NotSupported_WrongResourceReader_Type, text));
			}
			SkipString();
		}
		int num3 = _store.ReadInt32();
		if (num3 != 2 && num3 != 1)
		{
			throw new ArgumentException(SR.Format(SR.Arg_ResourceFileUnsupportedVersion, 2, num3));
		}
		_version = num3;
		_numResources = _store.ReadInt32();
		if (_numResources < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
		}
		int num4 = _store.ReadInt32();
		if (num4 < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
		}
		_typeTable = new Type[num4];
		_typeNamePositions = new int[num4];
		for (int i = 0; i < num4; i++)
		{
			_typeNamePositions[i] = (int)_store.BaseStream.Position;
			SkipString();
		}
		int num5 = (int)_store.BaseStream.Position & 7;
		if (num5 != 0)
		{
			for (int j = 0; j < 8 - num5; j++)
			{
				_store.ReadByte();
			}
		}
		if (_ums == null)
		{
			_nameHashes = new int[_numResources];
			for (int k = 0; k < _numResources; k++)
			{
				_nameHashes[k] = _store.ReadInt32();
			}
		}
		else
		{
			int num6 = 4 * _numResources;
			if (num6 < 0)
			{
				throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
			}
			_nameHashesPtr = (int*)_ums.PositionPointer;
			_ums.Seek(num6, SeekOrigin.Current);
			_ = _ums.PositionPointer;
		}
		if (_ums == null)
		{
			_namePositions = new int[_numResources];
			for (int l = 0; l < _numResources; l++)
			{
				int num7 = _store.ReadInt32();
				if (num7 < 0)
				{
					throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
				}
				_namePositions[l] = num7;
			}
		}
		else
		{
			int num8 = 4 * _numResources;
			if (num8 < 0)
			{
				throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
			}
			_namePositionsPtr = (int*)_ums.PositionPointer;
			_ums.Seek(num8, SeekOrigin.Current);
			_ = _ums.PositionPointer;
		}
		_dataSectionOffset = _store.ReadInt32();
		if (_dataSectionOffset < 0)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
		}
		_nameSectionOffset = _store.BaseStream.Position;
		if (_dataSectionOffset < _nameSectionOffset)
		{
			throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
		}
	}

	private Type FindType(int typeIndex)
	{
		if (!AllowCustomResourceTypes)
		{
			throw new NotSupportedException(SR.ResourceManager_ReflectionNotAllowed);
		}
		if (typeIndex < 0 || typeIndex >= _typeTable.Length)
		{
			throw new BadImageFormatException(SR.BadImageFormat_InvalidType);
		}
		return _typeTable[typeIndex] ?? UseReflectionToGetType(typeIndex);
	}

	[RequiresUnreferencedCode("The CustomResourceTypesSupport feature switch has been enabled for this app which is being trimmed. Custom readers as well as custom objects on the resources file are not observable by the trimmer and so required assemblies, types and members may be removed.")]
	private Type UseReflectionToGetType(int typeIndex)
	{
		long position = _store.BaseStream.Position;
		try
		{
			_store.BaseStream.Position = _typeNamePositions[typeIndex];
			string typeName = _store.ReadString();
			_typeTable[typeIndex] = Type.GetType(typeName, throwOnError: true);
			return _typeTable[typeIndex];
		}
		catch (FileNotFoundException innerException) when (!_permitDeserialization)
		{
			throw new NotSupportedException(SR.NotSupported_ResourceObjectSerialization, innerException);
		}
		finally
		{
			_store.BaseStream.Position = position;
		}
	}

	private string TypeNameFromTypeCode(ResourceTypeCode typeCode)
	{
		if (typeCode < ResourceTypeCode.StartOfUserTypes)
		{
			return "ResourceTypeCode." + typeCode;
		}
		int num = (int)(typeCode - 64);
		long position = _store.BaseStream.Position;
		try
		{
			_store.BaseStream.Position = _typeNamePositions[num];
			return _store.ReadString();
		}
		finally
		{
			_store.BaseStream.Position = position;
		}
	}
}
