using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Formats.Tar;

internal sealed class TarHeader
{
	internal Stream _dataStream;

	internal long _dataOffset;

	internal long _endOfHeaderAndDataAndBlockAlignment;

	internal TarEntryFormat _format;

	internal string _name;

	internal int _mode;

	internal int _uid;

	internal int _gid;

	internal long _size;

	internal DateTimeOffset _mTime;

	internal int _checksum;

	internal TarEntryType _typeFlag;

	internal string _linkName;

	internal string _magic;

	internal string _version;

	internal string _gName;

	internal string _uName;

	internal int _devMajor;

	internal int _devMinor;

	internal string _prefix;

	private Dictionary<string, string> _ea;

	internal DateTimeOffset _aTime;

	internal DateTimeOffset _cTime;

	internal Dictionary<string, string> ExtendedAttributes => _ea ?? (_ea = new Dictionary<string, string>());

	private static ReadOnlySpan<byte> UstarMagicBytes => "ustar\0"u8;

	private static ReadOnlySpan<byte> UstarVersionBytes => "00"u8;

	private static ReadOnlySpan<byte> GnuMagicBytes => "ustar "u8;

	private static ReadOnlySpan<byte> GnuVersionBytes => " \0"u8;

	internal TarHeader(TarEntryFormat format, string name = "", int mode = 0, DateTimeOffset mTime = default(DateTimeOffset), TarEntryType typeFlag = TarEntryType.RegularFile)
	{
		_format = format;
		_name = name;
		_mode = mode;
		_mTime = mTime;
		_typeFlag = typeFlag;
		_magic = GetMagicForFormat(format);
		_version = GetVersionForFormat(format);
		_dataOffset = -1L;
	}

	internal TarHeader(TarEntryFormat format, TarEntryType typeFlag, TarHeader other)
		: this(format, other._name, other._mode, other._mTime, typeFlag)
	{
		_uid = other._uid;
		_gid = other._gid;
		_size = other._size;
		_checksum = other._checksum;
		_linkName = other._linkName;
		_dataStream = other._dataStream;
	}

	internal void AddExtendedAttributes(IEnumerable<KeyValuePair<string, string>> existing)
	{
		foreach (KeyValuePair<string, string> item in existing)
		{
			int num = item.Key.AsSpan().IndexOfAny('=', '\n');
			if (num >= 0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.TarExtAttrDisallowedKeyChar, item.Key, (item.Key[num] == '\n') ? "\\n" : ((object)item.Key[num])));
			}
			if (item.Value.Contains('\n'))
			{
				throw new ArgumentException(System.SR.Format(System.SR.TarExtAttrDisallowedValueChar, item.Key, "\\n"));
			}
			if (_ea == null)
			{
				_ea = new Dictionary<string, string>();
			}
			_ea.Add(item.Key, item.Value);
		}
	}

	private static string GetMagicForFormat(TarEntryFormat format)
	{
		switch (format)
		{
		case TarEntryFormat.Ustar:
		case TarEntryFormat.Pax:
			return "ustar\0";
		case TarEntryFormat.Gnu:
			return "ustar ";
		default:
			return string.Empty;
		}
	}

	private static string GetVersionForFormat(TarEntryFormat format)
	{
		switch (format)
		{
		case TarEntryFormat.Ustar:
		case TarEntryFormat.Pax:
			return "00";
		case TarEntryFormat.Gnu:
			return " \0";
		default:
			return string.Empty;
		}
	}

	private static void SetDataOffset(TarHeader header, Stream archiveStream)
	{
		header._dataOffset = (archiveStream.CanSeek ? archiveStream.Position : (-1));
	}

	internal static TarHeader TryGetNextHeader(Stream archiveStream, bool copyData, TarEntryFormat initialFormat, bool processDataBlock)
	{
		Span<byte> span = stackalloc byte[512];
		archiveStream.ReadExactly(span);
		TarHeader tarHeader = TryReadAttributes(initialFormat, span, archiveStream);
		if ((tarHeader != null) & processDataBlock)
		{
			tarHeader.ProcessDataBlock(archiveStream, copyData);
		}
		return tarHeader;
	}

	internal static async ValueTask<TarHeader> TryGetNextHeaderAsync(Stream archiveStream, bool copyData, TarEntryFormat initialFormat, bool processDataBlock, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] rented = ArrayPool<byte>.Shared.Rent(512);
		Memory<byte> buffer = rented.AsMemory(0, 512);
		await archiveStream.ReadExactlyAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		TarHeader header = TryReadAttributes(initialFormat, buffer.Span, archiveStream);
		if ((header != null) & processDataBlock)
		{
			await header.ProcessDataBlockAsync(archiveStream, copyData, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		ArrayPool<byte>.Shared.Return(rented);
		return header;
	}

	private static TarHeader TryReadAttributes(TarEntryFormat initialFormat, ReadOnlySpan<byte> buffer, Stream archiveStream)
	{
		TarHeader tarHeader = TryReadCommonAttributes(buffer, initialFormat);
		if (tarHeader != null)
		{
			tarHeader.ReadMagicAttribute(buffer);
			if (tarHeader._format != TarEntryFormat.V7)
			{
				tarHeader.ReadVersionAttribute(buffer);
				tarHeader.ReadPosixAndGnuSharedAttributes(buffer);
				if (tarHeader._format == TarEntryFormat.Ustar)
				{
					tarHeader.ReadUstarAttributes(buffer);
				}
				else if (tarHeader._format == TarEntryFormat.Gnu)
				{
					tarHeader.ReadGnuAttributes(buffer);
				}
			}
			SetDataOffset(tarHeader, archiveStream);
		}
		return tarHeader;
	}

	internal void ReplaceNormalAttributesWithExtended(Dictionary<string, string> dictionaryFromExtendedAttributesHeader)
	{
		if (dictionaryFromExtendedAttributesHeader == null || dictionaryFromExtendedAttributesHeader.Count == 0)
		{
			return;
		}
		AddExtendedAttributes(dictionaryFromExtendedAttributesHeader);
		if (ExtendedAttributes.TryGetValue("path", out var value))
		{
			_name = value;
		}
		if (ExtendedAttributes.TryGetValue("linkpath", out var value2))
		{
			_linkName = value2;
		}
		if (TarHelpers.TryGetDateTimeOffsetFromTimestampString(ExtendedAttributes, "mtime", out var dateTimeOffset))
		{
			_mTime = dateTimeOffset;
		}
		if (TarHelpers.TryGetStringAsBaseTenInteger(ExtendedAttributes, "mode", out var baseTenInteger))
		{
			_mode = baseTenInteger;
		}
		if (TarHelpers.TryGetStringAsBaseTenLong(ExtendedAttributes, "size", out var baseTenLong))
		{
			if (baseTenLong < 0)
			{
				throw new InvalidDataException(System.SR.Format(System.SR.TarSizeFieldNegative));
			}
			_size = baseTenLong;
		}
		if (TarHelpers.TryGetStringAsBaseTenInteger(ExtendedAttributes, "uid", out var baseTenInteger2))
		{
			_uid = baseTenInteger2;
		}
		if (TarHelpers.TryGetStringAsBaseTenInteger(ExtendedAttributes, "gid", out var baseTenInteger3))
		{
			_gid = baseTenInteger3;
		}
		if (ExtendedAttributes.TryGetValue("uname", out var value3))
		{
			_uName = value3;
		}
		if (ExtendedAttributes.TryGetValue("gname", out var value4))
		{
			_gName = value4;
		}
		if (TarHelpers.TryGetStringAsBaseTenInteger(ExtendedAttributes, "devmajor", out var baseTenInteger4))
		{
			_devMajor = baseTenInteger4;
		}
		if (TarHelpers.TryGetStringAsBaseTenInteger(ExtendedAttributes, "devminor", out var baseTenInteger5))
		{
			_devMinor = baseTenInteger5;
		}
	}

	internal void ProcessDataBlock(Stream archiveStream, bool copyData)
	{
		bool flag = true;
		switch (_typeFlag)
		{
		case TarEntryType.GlobalExtendedAttributes:
		case TarEntryType.ExtendedAttributes:
			ReadExtendedAttributesBlock(archiveStream);
			break;
		case TarEntryType.LongLink:
		case TarEntryType.LongPath:
			ReadGnuLongPathDataBlock(archiveStream);
			break;
		case TarEntryType.HardLink:
		case TarEntryType.SymbolicLink:
		case TarEntryType.CharacterDevice:
		case TarEntryType.BlockDevice:
		case TarEntryType.Directory:
		case TarEntryType.Fifo:
			if (_size > 0)
			{
				throw new InvalidDataException(System.SR.Format(System.SR.TarSizeFieldTooLargeForEntryType, _typeFlag));
			}
			break;
		default:
			_dataStream = GetDataStream(archiveStream, copyData);
			if (_dataStream is SeekableSubReadStream)
			{
				TarHelpers.AdvanceStream(archiveStream, _size);
			}
			else if (_dataStream is SubReadStream)
			{
				flag = false;
			}
			break;
		}
		if (flag)
		{
			if (_size > 0)
			{
				TarHelpers.SkipBlockAlignmentPadding(archiveStream, _size);
			}
			if (archiveStream.CanSeek)
			{
				_endOfHeaderAndDataAndBlockAlignment = archiveStream.Position;
			}
		}
	}

	private async Task ProcessDataBlockAsync(Stream archiveStream, bool copyData, CancellationToken cancellationToken)
	{
		bool skipBlockAlignmentPadding = true;
		switch (_typeFlag)
		{
		case TarEntryType.GlobalExtendedAttributes:
		case TarEntryType.ExtendedAttributes:
			await ReadExtendedAttributesBlockAsync(archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			break;
		case TarEntryType.LongLink:
		case TarEntryType.LongPath:
			await ReadGnuLongPathDataBlockAsync(archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			break;
		case TarEntryType.HardLink:
		case TarEntryType.SymbolicLink:
		case TarEntryType.CharacterDevice:
		case TarEntryType.BlockDevice:
		case TarEntryType.Directory:
		case TarEntryType.Fifo:
			if (_size > 0)
			{
				throw new InvalidDataException(System.SR.Format(System.SR.TarSizeFieldTooLargeForEntryType, _typeFlag));
			}
			break;
		default:
			_dataStream = await GetDataStreamAsync(archiveStream, copyData, _size, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_dataStream is SeekableSubReadStream)
			{
				await TarHelpers.AdvanceStreamAsync(archiveStream, _size, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else if (_dataStream is SubReadStream)
			{
				skipBlockAlignmentPadding = false;
			}
			break;
		}
		if (skipBlockAlignmentPadding)
		{
			if (_size > 0)
			{
				await TarHelpers.SkipBlockAlignmentPaddingAsync(archiveStream, _size, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (archiveStream.CanSeek)
			{
				_endOfHeaderAndDataAndBlockAlignment = archiveStream.Position;
			}
		}
	}

	private Stream GetDataStream(Stream archiveStream, bool copyData)
	{
		if (_size == 0L)
		{
			return null;
		}
		if (copyData)
		{
			MemoryStream memoryStream = new MemoryStream();
			TarHelpers.CopyBytes(archiveStream, memoryStream, _size);
			memoryStream.Position = 0L;
			return memoryStream;
		}
		if (!archiveStream.CanSeek)
		{
			return new SubReadStream(archiveStream, 0L, _size);
		}
		return new SeekableSubReadStream(archiveStream, archiveStream.Position, _size);
	}

	private static async ValueTask<Stream> GetDataStreamAsync(Stream archiveStream, bool copyData, long size, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (size == 0L)
		{
			return null;
		}
		if (copyData)
		{
			MemoryStream copiedData = new MemoryStream();
			await TarHelpers.CopyBytesAsync(archiveStream, copiedData, size, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			copiedData.Position = 0L;
			return copiedData;
		}
		return archiveStream.CanSeek ? new SeekableSubReadStream(archiveStream, archiveStream.Position, size) : new SubReadStream(archiveStream, 0L, size);
	}

	private static TarHeader TryReadCommonAttributes(ReadOnlySpan<byte> buffer, TarEntryFormat initialFormat)
	{
		ReadOnlySpan<byte> buffer2 = buffer.Slice(148, 8);
		if (TarHelpers.IsAllNullBytes(buffer2))
		{
			return null;
		}
		int num = (int)TarHelpers.ParseOctal<uint>(buffer2);
		if (num == 0)
		{
			return null;
		}
		long num2 = TarHelpers.ParseNumeric<long>(buffer.Slice(124, 12));
		if (num2 < 0)
		{
			throw new InvalidDataException(System.SR.Format(System.SR.TarSizeFieldNegative));
		}
		TarHeader tarHeader = new TarHeader(initialFormat, TarHelpers.ParseUtf8String(buffer.Slice(0, 100)), TarHelpers.ParseNumeric<int>(buffer.Slice(100, 8)), ParseAsTimestamp(buffer.Slice(136, 12)), (TarEntryType)buffer[156])
		{
			_checksum = num,
			_size = num2,
			_uid = TarHelpers.ParseNumeric<int>(buffer.Slice(108, 8)),
			_gid = TarHelpers.ParseNumeric<int>(buffer.Slice(116, 8)),
			_linkName = TarHelpers.ParseUtf8String(buffer.Slice(157, 100))
		};
		if (tarHeader._format == TarEntryFormat.Unknown)
		{
			TarHeader tarHeader2 = tarHeader;
			TarEntryFormat format;
			switch (tarHeader._typeFlag)
			{
			case TarEntryType.GlobalExtendedAttributes:
			case TarEntryType.ExtendedAttributes:
				format = TarEntryFormat.Pax;
				break;
			case TarEntryType.DirectoryList:
			case TarEntryType.LongLink:
			case TarEntryType.LongPath:
			case TarEntryType.MultiVolume:
			case TarEntryType.RenamedOrSymlinked:
			case TarEntryType.TapeVolume:
				format = TarEntryFormat.Gnu;
				break;
			case TarEntryType.V7RegularFile:
				format = TarEntryFormat.V7;
				break;
			case TarEntryType.SparseFile:
				throw new NotSupportedException(System.SR.Format(System.SR.TarEntryTypeNotSupported, tarHeader._typeFlag));
			default:
				format = ((tarHeader._typeFlag != TarEntryType.RegularFile) ? TarEntryFormat.V7 : TarEntryFormat.Ustar);
				break;
			}
			tarHeader2._format = format;
		}
		return tarHeader;
	}

	private void ReadMagicAttribute(ReadOnlySpan<byte> buffer)
	{
		ReadOnlySpan<byte> readOnlySpan = buffer.Slice(257, 6);
		if (TarHelpers.IsAllNullBytes(readOnlySpan))
		{
			_format = TarEntryFormat.V7;
		}
		else if (readOnlySpan.SequenceEqual(GnuMagicBytes))
		{
			_magic = "ustar ";
			_format = TarEntryFormat.Gnu;
		}
		else if (readOnlySpan.SequenceEqual(UstarMagicBytes))
		{
			_magic = "ustar\0";
			if (_format == TarEntryFormat.V7)
			{
				_format = TarEntryFormat.Ustar;
			}
		}
		else
		{
			_magic = Encoding.ASCII.GetString(readOnlySpan);
		}
	}

	private void ReadVersionAttribute(ReadOnlySpan<byte> buffer)
	{
		if (_format == TarEntryFormat.V7)
		{
			return;
		}
		ReadOnlySpan<byte> readOnlySpan = buffer.Slice(263, 2);
		switch (_format)
		{
		case TarEntryFormat.Ustar:
		case TarEntryFormat.Pax:
			if (!readOnlySpan.SequenceEqual(UstarVersionBytes))
			{
				if (!readOnlySpan.SequenceEqual(GnuVersionBytes))
				{
					throw new InvalidDataException(System.SR.Format(System.SR.TarPosixFormatExpected, _name));
				}
				_version = " \0";
			}
			else
			{
				_version = "00";
			}
			break;
		case TarEntryFormat.Gnu:
			if (!readOnlySpan.SequenceEqual(GnuVersionBytes))
			{
				if (!readOnlySpan.SequenceEqual(UstarVersionBytes))
				{
					throw new InvalidDataException(System.SR.Format(System.SR.TarGnuFormatExpected, _name));
				}
				_version = "00";
			}
			else
			{
				_version = " \0";
			}
			break;
		default:
			_version = Encoding.ASCII.GetString(readOnlySpan);
			break;
		}
	}

	private void ReadPosixAndGnuSharedAttributes(ReadOnlySpan<byte> buffer)
	{
		_uName = TarHelpers.ParseUtf8String(buffer.Slice(265, 32));
		_gName = TarHelpers.ParseUtf8String(buffer.Slice(297, 32));
		TarEntryType typeFlag = _typeFlag;
		if (typeFlag - 51 <= (TarEntryType)1)
		{
			_devMajor = TarHelpers.ParseNumeric<int>(buffer.Slice(329, 8));
			_devMinor = TarHelpers.ParseNumeric<int>(buffer.Slice(337, 8));
		}
	}

	private void ReadGnuAttributes(ReadOnlySpan<byte> buffer)
	{
		_aTime = ParseAsTimestamp(buffer.Slice(345, 12));
		_cTime = ParseAsTimestamp(buffer.Slice(357, 12));
	}

	private static DateTimeOffset ParseAsTimestamp(ReadOnlySpan<byte> buffer)
	{
		if (!buffer.ContainsAnyExcept((byte)0))
		{
			return default(DateTimeOffset);
		}
		return TarHelpers.GetDateTimeOffsetFromSecondsSinceEpoch(TarHelpers.ParseNumeric<long>(buffer));
	}

	private void ReadUstarAttributes(ReadOnlySpan<byte> buffer)
	{
		_prefix = TarHelpers.ParseUtf8String(buffer.Slice(345, 155));
		if (!string.IsNullOrEmpty(_prefix))
		{
			_name = _prefix + "/" + _name;
		}
	}

	private void ReadExtendedAttributesBlock(Stream archiveStream)
	{
		long size = _size;
		if (size != 0L)
		{
			ValidateSize();
			byte[] array = null;
			Span<byte> span = (((ulong)size > 256uL) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent((int)size))) : stackalloc byte[256]);
			Span<byte> span2 = span;
			span2 = span2.Slice(0, (int)size);
			archiveStream.ReadExactly(span2);
			ReadExtendedAttributesFromBuffer(span2, _name);
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	private async ValueTask ReadExtendedAttributesBlockAsync(Stream archiveStream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_size != 0L)
		{
			ValidateSize();
			byte[] buffer = ArrayPool<byte>.Shared.Rent((int)_size);
			Memory<byte> memory = buffer.AsMemory(0, (int)_size);
			await archiveStream.ReadExactlyAsync(memory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ReadExtendedAttributesFromBuffer(memory.Span, _name);
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	private void ValidateSize()
	{
		if ((uint)_size > (uint)Array.MaxLength)
		{
			ThrowSizeFieldTooLarge();
		}
		[DoesNotReturn]
		void ThrowSizeFieldTooLarge()
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.TarSizeFieldTooLargeForEntryType, _typeFlag.ToString()));
		}
	}

	private void ReadExtendedAttributesFromBuffer(ReadOnlySpan<byte> buffer, string name)
	{
		string key;
		string value;
		while (TryGetNextExtendedAttribute(ref buffer, out key, out value))
		{
			if (!ExtendedAttributes.TryAdd(key, value))
			{
				throw new InvalidDataException(System.SR.Format(System.SR.TarDuplicateExtendedAttribute, name));
			}
		}
		if (buffer.Length > 0)
		{
			throw new InvalidDataException(System.SR.Format(System.SR.ExtHeaderInvalidRecords));
		}
	}

	private void ReadGnuLongPathDataBlock(Stream archiveStream)
	{
		long size = _size;
		if (size != 0L)
		{
			ValidateSize();
			byte[] array = null;
			Span<byte> span = (((ulong)size > 256uL) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent((int)size))) : stackalloc byte[256]);
			Span<byte> span2 = span;
			span2 = span2.Slice(0, (int)size);
			archiveStream.ReadExactly(span2);
			ReadGnuLongPathDataFromBuffer(span2);
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	private async ValueTask ReadGnuLongPathDataBlockAsync(Stream archiveStream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (_size != 0L)
		{
			ValidateSize();
			byte[] buffer = ArrayPool<byte>.Shared.Rent((int)_size);
			Memory<byte> memory = buffer.AsMemory(0, (int)_size);
			await archiveStream.ReadExactlyAsync(memory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ReadGnuLongPathDataFromBuffer(memory.Span);
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	private void ReadGnuLongPathDataFromBuffer(ReadOnlySpan<byte> buffer)
	{
		string text = TarHelpers.ParseUtf8String(buffer);
		if (_typeFlag == TarEntryType.LongLink)
		{
			_linkName = text;
		}
		else if (_typeFlag == TarEntryType.LongPath)
		{
			_name = text;
		}
	}

	private static bool TryGetNextExtendedAttribute(ref ReadOnlySpan<byte> buffer, [NotNullWhen(true)] out string key, [NotNullWhen(true)] out string value)
	{
		key = null;
		value = null;
		int num = buffer.IndexOf((byte)10);
		if (num < 0)
		{
			return false;
		}
		ReadOnlySpan<byte> span = buffer.Slice(0, num);
		int num2 = span.IndexOf((byte)32);
		if (num2 < 0)
		{
			return false;
		}
		if (!int.TryParse(buffer.Slice(0, num2), NumberStyles.None, CultureInfo.InvariantCulture, out var result) || result != span.Length + 1)
		{
			return false;
		}
		span = span.Slice(num2 + 1);
		int num3 = span.IndexOf((byte)61);
		if (num3 < 0)
		{
			return false;
		}
		ReadOnlySpan<byte> bytes = span.Slice(0, num3);
		ReadOnlySpan<byte> bytes2 = span.Slice(num3 + 1);
		key = Encoding.UTF8.GetString(bytes);
		value = Encoding.UTF8.GetString(bytes2);
		buffer = buffer.Slice(num + 1);
		return true;
	}

	private void WriteWithSeekableDataStream(TarEntryFormat format, Stream archiveStream, Span<byte> buffer)
	{
		_size = GetTotalDataBytesToWrite();
		WriteFieldsToBuffer(format, buffer);
		archiveStream.Write(buffer);
		if (_dataStream != null)
		{
			WriteData(archiveStream, _dataStream);
		}
	}

	private async Task WriteWithSeekableDataStreamAsync(TarEntryFormat format, Stream archiveStream, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		_size = GetTotalDataBytesToWrite();
		WriteFieldsToBuffer(format, buffer.Span);
		await archiveStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (_dataStream != null)
		{
			await WriteDataAsync(archiveStream, _dataStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void WriteWithUnseekableDataStream(TarEntryFormat format, Stream destinationStream, Span<byte> buffer, bool shouldAdvanceToEnd)
	{
		long position = destinationStream.Position;
		ushort num;
		switch (format)
		{
		case TarEntryFormat.V7:
			num = 512;
			break;
		case TarEntryFormat.Ustar:
		case TarEntryFormat.Pax:
			num = 512;
			break;
		case TarEntryFormat.Gnu:
			num = 512;
			break;
		default:
			throw new ArgumentOutOfRangeException("format");
		}
		ushort num2 = num;
		long num3 = (_dataOffset = position + num2);
		destinationStream.Seek(num2, SeekOrigin.Current);
		_dataStream.CopyTo(destinationStream);
		long position2 = destinationStream.Position;
		_size = position2 - num3;
		WriteEmptyPadding(destinationStream);
		long position3 = destinationStream.Position;
		destinationStream.Position = position;
		WriteFieldsToBuffer(format, buffer);
		destinationStream.Write(buffer);
		if (shouldAdvanceToEnd)
		{
			destinationStream.Position = position3;
		}
	}

	private async Task WriteWithUnseekableDataStreamAsync(TarEntryFormat format, Stream destinationStream, Memory<byte> buffer, bool shouldAdvanceToEnd, CancellationToken cancellationToken)
	{
		long headerStartPosition = destinationStream.Position;
		ushort num;
		switch (format)
		{
		case TarEntryFormat.V7:
			num = 512;
			break;
		case TarEntryFormat.Ustar:
		case TarEntryFormat.Pax:
			num = 512;
			break;
		case TarEntryFormat.Gnu:
			num = 512;
			break;
		default:
			throw new ArgumentOutOfRangeException("format");
		}
		ushort num2 = num;
		long dataStartPosition = (_dataOffset = headerStartPosition + num2);
		destinationStream.Seek(num2, SeekOrigin.Current);
		await _dataStream.CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		long position = destinationStream.Position;
		_size = position - dataStartPosition;
		await WriteEmptyPaddingAsync(destinationStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		long endOfHeaderPosition = destinationStream.Position;
		destinationStream.Position = headerStartPosition;
		WriteFieldsToBuffer(format, buffer.Span);
		await destinationStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (shouldAdvanceToEnd)
		{
			destinationStream.Position = endOfHeaderPosition;
		}
	}

	private void WriteV7FieldsToBuffer(Span<byte> buffer)
	{
		TarEntryType correctTypeFlagForFormat = TarHelpers.GetCorrectTypeFlagForFormat(TarEntryFormat.V7, _typeFlag);
		int num = WriteName(buffer);
		num += WriteCommonFields(buffer, correctTypeFlagForFormat);
		_checksum = WriteChecksum(num, buffer);
	}

	private void WriteUstarFieldsToBuffer(Span<byte> buffer)
	{
		TarEntryType correctTypeFlagForFormat = TarHelpers.GetCorrectTypeFlagForFormat(TarEntryFormat.Ustar, _typeFlag);
		int num = WriteUstarName(buffer);
		num += WriteCommonFields(buffer, correctTypeFlagForFormat);
		num += WritePosixMagicAndVersion(buffer);
		num += WritePosixAndGnuSharedFields(buffer);
		_checksum = WriteChecksum(num, buffer);
	}

	internal void WriteAsPaxGlobalExtendedAttributes(Stream archiveStream, Span<byte> buffer, int globalExtendedAttributesEntryNumber)
	{
		VerifyGlobalExtendedAttributesDataIsValid(globalExtendedAttributesEntryNumber);
		WriteAsPaxExtendedAttributes(archiveStream, buffer, ExtendedAttributes, isGea: true, globalExtendedAttributesEntryNumber);
	}

	internal Task WriteAsPaxGlobalExtendedAttributesAsync(Stream archiveStream, Memory<byte> buffer, int globalExtendedAttributesEntryNumber, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled<int>(cancellationToken);
		}
		VerifyGlobalExtendedAttributesDataIsValid(globalExtendedAttributesEntryNumber);
		return WriteAsPaxExtendedAttributesAsync(archiveStream, buffer, ExtendedAttributes, isGea: true, globalExtendedAttributesEntryNumber, cancellationToken);
	}

	private void VerifyGlobalExtendedAttributesDataIsValid(int globalExtendedAttributesEntryNumber)
	{
	}

	internal void WriteAsV7(Stream archiveStream, Span<byte> buffer)
	{
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				WriteWithUnseekableDataStream(TarEntryFormat.V7, archiveStream, buffer, shouldAdvanceToEnd: true);
				return;
			}
		}
		WriteWithSeekableDataStream(TarEntryFormat.V7, archiveStream, buffer);
	}

	internal Task WriteAsV7Async(Stream archiveStream, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				return WriteWithUnseekableDataStreamAsync(TarEntryFormat.V7, archiveStream, buffer, shouldAdvanceToEnd: true, cancellationToken);
			}
		}
		return WriteWithSeekableDataStreamAsync(TarEntryFormat.V7, archiveStream, buffer, cancellationToken);
	}

	internal void WriteAsUstar(Stream archiveStream, Span<byte> buffer)
	{
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				WriteWithUnseekableDataStream(TarEntryFormat.Ustar, archiveStream, buffer, shouldAdvanceToEnd: true);
				return;
			}
		}
		WriteWithSeekableDataStream(TarEntryFormat.Ustar, archiveStream, buffer);
	}

	internal Task WriteAsUstarAsync(Stream archiveStream, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				return WriteWithUnseekableDataStreamAsync(TarEntryFormat.Ustar, archiveStream, buffer, shouldAdvanceToEnd: true, cancellationToken);
			}
		}
		return WriteWithSeekableDataStreamAsync(TarEntryFormat.Ustar, archiveStream, buffer, cancellationToken);
	}

	internal void WriteAsPax(Stream archiveStream, Span<byte> buffer)
	{
		TarHeader tarHeader = new TarHeader(TarEntryFormat.Pax);
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					WriteWithUnseekableDataStream(TarEntryFormat.Pax, memoryStream, buffer, shouldAdvanceToEnd: false);
					memoryStream.Position = 0L;
					buffer.Clear();
					CollectExtendedAttributesFromStandardFieldsIfNeeded();
					tarHeader.WriteAsPaxExtendedAttributes(archiveStream, buffer, ExtendedAttributes, isGea: false, -1);
					buffer.Clear();
					memoryStream.CopyTo(archiveStream);
					return;
				}
			}
		}
		_size = GetTotalDataBytesToWrite();
		CollectExtendedAttributesFromStandardFieldsIfNeeded();
		tarHeader.WriteAsPaxExtendedAttributes(archiveStream, buffer, ExtendedAttributes, isGea: false, -1);
		buffer.Clear();
		WriteWithSeekableDataStream(TarEntryFormat.Pax, archiveStream, buffer);
	}

	internal async Task WriteAsPaxAsync(Stream archiveStream, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		TarHeader extendedAttributesHeader = new TarHeader(TarEntryFormat.Pax);
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				using (MemoryStream tempStream = new MemoryStream())
				{
					await WriteWithUnseekableDataStreamAsync(TarEntryFormat.Pax, tempStream, buffer, shouldAdvanceToEnd: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					tempStream.Position = 0L;
					buffer.Span.Clear();
					CollectExtendedAttributesFromStandardFieldsIfNeeded();
					await extendedAttributesHeader.WriteAsPaxExtendedAttributesAsync(archiveStream, buffer, ExtendedAttributes, isGea: false, -1, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					buffer.Span.Clear();
					await tempStream.CopyToAsync(archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				return;
			}
		}
		_size = GetTotalDataBytesToWrite();
		CollectExtendedAttributesFromStandardFieldsIfNeeded();
		await extendedAttributesHeader.WriteAsPaxExtendedAttributesAsync(archiveStream, buffer, ExtendedAttributes, isGea: false, -1, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		buffer.Span.Clear();
		await WriteWithSeekableDataStreamAsync(TarEntryFormat.Pax, archiveStream, buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private bool IsLinkNameTooLongForRegularField()
	{
		if (_linkName != null)
		{
			return Encoding.UTF8.GetByteCount(_linkName) + 1 > 100;
		}
		return false;
	}

	private bool IsNameTooLongForRegularField()
	{
		return Encoding.UTF8.GetByteCount(_name) > 100;
	}

	internal void WriteAsGnu(Stream archiveStream, Span<byte> buffer)
	{
		if (IsLinkNameTooLongForRegularField())
		{
			GetGnuLongLinkMetadataHeader().WriteWithSeekableDataStream(TarEntryFormat.Gnu, archiveStream, buffer);
			buffer.Clear();
		}
		if (IsNameTooLongForRegularField())
		{
			GetGnuLongPathMetadataHeader().WriteWithSeekableDataStream(TarEntryFormat.Gnu, archiveStream, buffer);
			buffer.Clear();
		}
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				WriteWithUnseekableDataStream(TarEntryFormat.Gnu, archiveStream, buffer, shouldAdvanceToEnd: true);
				return;
			}
		}
		WriteWithSeekableDataStream(TarEntryFormat.Gnu, archiveStream, buffer);
	}

	internal async Task WriteAsGnuAsync(Stream archiveStream, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (IsLinkNameTooLongForRegularField())
		{
			await GetGnuLongLinkMetadataHeader().WriteWithSeekableDataStreamAsync(TarEntryFormat.Gnu, archiveStream, buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			buffer.Span.Clear();
		}
		if (IsNameTooLongForRegularField())
		{
			await GetGnuLongPathMetadataHeader().WriteWithSeekableDataStreamAsync(TarEntryFormat.Gnu, archiveStream, buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			buffer.Span.Clear();
		}
		if (archiveStream.CanSeek)
		{
			Stream dataStream = _dataStream;
			if (dataStream != null && !dataStream.CanSeek)
			{
				await WriteWithUnseekableDataStreamAsync(TarEntryFormat.Gnu, archiveStream, buffer, shouldAdvanceToEnd: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				return;
			}
		}
		await WriteWithSeekableDataStreamAsync(TarEntryFormat.Gnu, archiveStream, buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static MemoryStream GetLongMetadataStream(string text)
	{
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(Encoding.UTF8.GetBytes(text));
		memoryStream.WriteByte(0);
		memoryStream.Position = 0L;
		return memoryStream;
	}

	private TarHeader GetGnuLongLinkMetadataHeader()
	{
		return GetGnuLongMetadataHeader(GetLongMetadataStream(_linkName), TarEntryType.LongLink);
	}

	private TarHeader GetGnuLongPathMetadataHeader()
	{
		return GetGnuLongMetadataHeader(GetLongMetadataStream(_name), TarEntryType.LongPath);
	}

	private static TarHeader GetGnuLongMetadataHeader(MemoryStream dataStream, TarEntryType entryType)
	{
		return new TarHeader(TarEntryFormat.Gnu)
		{
			_name = "././@LongLink",
			_mode = TarHelpers.GetDefaultMode(entryType),
			_uid = 0,
			_gid = 0,
			_mTime = DateTimeOffset.UnixEpoch,
			_typeFlag = entryType,
			_dataStream = dataStream,
			_uName = "root",
			_gName = "root",
			_aTime = default(DateTimeOffset),
			_cTime = default(DateTimeOffset)
		};
	}

	private void WriteGnuFieldsToBuffer(Span<byte> buffer)
	{
		int num = WriteName(buffer);
		num += WriteCommonFields(buffer, TarHelpers.GetCorrectTypeFlagForFormat(TarEntryFormat.Gnu, _typeFlag));
		num += WriteGnuMagicAndVersion(buffer);
		num += WritePosixAndGnuSharedFields(buffer);
		num += WriteGnuFields(buffer);
		_checksum = WriteChecksum(num, buffer);
	}

	private void WriteAsPaxExtendedAttributes(Stream archiveStream, Span<byte> buffer, Dictionary<string, string> extendedAttributes, bool isGea, int globalExtendedAttributesEntryNumber)
	{
		WriteAsPaxExtendedAttributesShared(isGea, globalExtendedAttributesEntryNumber, extendedAttributes);
		WriteWithSeekableDataStream(TarEntryFormat.Pax, archiveStream, buffer);
	}

	private Task WriteAsPaxExtendedAttributesAsync(Stream archiveStream, Memory<byte> buffer, Dictionary<string, string> extendedAttributes, bool isGea, int globalExtendedAttributesEntryNumber, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		WriteAsPaxExtendedAttributesShared(isGea, globalExtendedAttributesEntryNumber, extendedAttributes);
		return WriteWithSeekableDataStreamAsync(TarEntryFormat.Pax, archiveStream, buffer, cancellationToken);
	}

	private void WriteAsPaxExtendedAttributesShared(bool isGea, int globalExtendedAttributesEntryNumber, Dictionary<string, string> extendedAttributes)
	{
		_dataStream = GenerateExtendedAttributesDataStream(extendedAttributes);
		_name = (isGea ? GenerateGlobalExtendedAttributeName(globalExtendedAttributesEntryNumber) : GenerateExtendedAttributeName());
		_mode = TarHelpers.GetDefaultMode(_typeFlag);
		_typeFlag = (isGea ? TarEntryType.GlobalExtendedAttributes : TarEntryType.ExtendedAttributes);
	}

	private void WritePaxFieldsToBuffer(Span<byte> buffer)
	{
		int num = WriteName(buffer);
		num += WriteCommonFields(buffer, TarHelpers.GetCorrectTypeFlagForFormat(TarEntryFormat.Pax, _typeFlag));
		num += WritePosixMagicAndVersion(buffer);
		num += WritePosixAndGnuSharedFields(buffer);
		_checksum = WriteChecksum(num, buffer);
	}

	private void WriteFieldsToBuffer(TarEntryFormat format, Span<byte> buffer)
	{
		switch (format)
		{
		case TarEntryFormat.V7:
			WriteV7FieldsToBuffer(buffer);
			break;
		case TarEntryFormat.Ustar:
			WriteUstarFieldsToBuffer(buffer);
			break;
		case TarEntryFormat.Pax:
			WritePaxFieldsToBuffer(buffer);
			break;
		case TarEntryFormat.Gnu:
			WriteGnuFieldsToBuffer(buffer);
			break;
		}
	}

	private int WriteName(Span<byte> buffer)
	{
		ReadOnlySpan<char> text = _name.AsSpan();
		if (GetUtf8TextLength(text) > 100)
		{
			if (_format == TarEntryFormat.V7)
			{
				throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "Name"), "entry");
			}
			text = text[..GetUtf16TruncatedTextLength(text, 100)];
		}
		return WriteAsUtf8String(text, buffer.Slice(0, 100));
	}

	private int WriteUstarName(Span<byte> buffer)
	{
		if (GetUtf8TextLength(_name.AsSpan()) > 256)
		{
			throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "Name"), "entry");
		}
		Span<byte> bytes = stackalloc byte[256];
		ReadOnlySpan<byte> readOnlySpan = bytes[..Encoding.UTF8.GetBytes(_name.AsSpan(), bytes)];
		if (readOnlySpan.Length <= 100)
		{
			return WriteLeftAlignedBytesAndGetChecksum(readOnlySpan, buffer.Slice(0, 100));
		}
		int num = readOnlySpan.LastIndexOfAny(System.IO.PathInternal.Utf8DirectorySeparators);
		ReadOnlySpan<byte> bytesToWrite;
		ReadOnlySpan<byte> readOnlySpan2;
		if (num < 1)
		{
			bytesToWrite = readOnlySpan;
			readOnlySpan2 = default(ReadOnlySpan<byte>);
		}
		else
		{
			bytesToWrite = readOnlySpan.Slice(num + 1);
			readOnlySpan2 = readOnlySpan.Slice(0, num);
		}
		while (readOnlySpan2.Length - bytesToWrite.Length > 155)
		{
			num = readOnlySpan2.LastIndexOfAny(System.IO.PathInternal.Utf8DirectorySeparators);
			if (num < 1)
			{
				break;
			}
			bytesToWrite = readOnlySpan.Slice(num + 1);
			readOnlySpan2 = readOnlySpan.Slice(0, num);
		}
		if (readOnlySpan2.Length <= 155 && bytesToWrite.Length <= 100)
		{
			return WriteLeftAlignedBytesAndGetChecksum(readOnlySpan2, buffer.Slice(345, 155)) + WriteLeftAlignedBytesAndGetChecksum(bytesToWrite, buffer.Slice(0, 100));
		}
		throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "Name"), "entry");
	}

	private int WriteCommonFields(Span<byte> buffer, TarEntryType actualEntryType)
	{
		int num = 0;
		if (_mode >= 0)
		{
			num += FormatNumeric(_mode, buffer.Slice(100, 8));
		}
		if (_uid >= 0)
		{
			num += FormatNumeric(_uid, buffer.Slice(108, 8));
		}
		if (_gid >= 0)
		{
			num += FormatNumeric(_gid, buffer.Slice(116, 8));
		}
		if (_size >= 0)
		{
			num += FormatNumeric(_size, buffer.Slice(124, 12));
		}
		num += WriteAsTimestamp(_mTime, buffer.Slice(136, 12));
		char c = (char)actualEntryType;
		buffer[156] = (byte)c;
		num += c;
		if (!string.IsNullOrEmpty(_linkName))
		{
			ReadOnlySpan<char> text = _linkName.AsSpan();
			if (GetUtf8TextLength(text) > 100)
			{
				TarEntryFormat format = _format;
				if (format != TarEntryFormat.Pax && format != TarEntryFormat.Gnu)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "LinkName"), "entry");
				}
				text = text[..GetUtf16TruncatedTextLength(text, 100)];
			}
			num += WriteAsUtf8String(text, buffer.Slice(157, 100));
		}
		return num;
	}

	public long GetTotalDataBytesToWrite()
	{
		if (_dataStream == null)
		{
			return 0L;
		}
		long length = _dataStream.Length;
		long position = _dataStream.Position;
		if (position >= length)
		{
			return 0L;
		}
		return length - position;
	}

	private static int WritePosixMagicAndVersion(Span<byte> buffer)
	{
		return WriteLeftAlignedBytesAndGetChecksum(UstarMagicBytes, buffer.Slice(257, 6)) + WriteLeftAlignedBytesAndGetChecksum(UstarVersionBytes, buffer.Slice(263, 2));
	}

	private static int WriteGnuMagicAndVersion(Span<byte> buffer)
	{
		return WriteLeftAlignedBytesAndGetChecksum(GnuMagicBytes, buffer.Slice(257, 6)) + WriteLeftAlignedBytesAndGetChecksum(GnuVersionBytes, buffer.Slice(263, 2));
	}

	private int WritePosixAndGnuSharedFields(Span<byte> buffer)
	{
		int num = 0;
		if (!string.IsNullOrEmpty(_uName))
		{
			ReadOnlySpan<char> text = _uName.AsSpan();
			if (GetUtf8TextLength(text) > 32)
			{
				if (_format != TarEntryFormat.Pax)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "UserName"), "entry");
				}
				text = text[..GetUtf16TruncatedTextLength(text, 32)];
			}
			num += WriteAsUtf8String(text, buffer.Slice(265, 32));
		}
		if (!string.IsNullOrEmpty(_gName))
		{
			ReadOnlySpan<char> text2 = _gName.AsSpan();
			if (GetUtf8TextLength(text2) > 32)
			{
				if (_format != TarEntryFormat.Pax)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TarEntryFieldExceedsMaxLength, "GroupName"), "entry");
				}
				text2 = text2[..GetUtf16TruncatedTextLength(text2, 32)];
			}
			num += WriteAsUtf8String(text2, buffer.Slice(297, 32));
		}
		if (_devMajor > 0)
		{
			num += FormatNumeric(_devMajor, buffer.Slice(329, 8));
		}
		if (_devMinor > 0)
		{
			num += FormatNumeric(_devMinor, buffer.Slice(337, 8));
		}
		return num;
	}

	private int WriteGnuFields(Span<byte> buffer)
	{
		int num = 0;
		TarEntryType typeFlag = _typeFlag;
		if (typeFlag != TarEntryType.LongLink && typeFlag != TarEntryType.LongPath)
		{
			num += WriteAsTimestamp(_aTime, buffer.Slice(345, 12));
			num += WriteAsTimestamp(_cTime, buffer.Slice(357, 12));
		}
		return num;
	}

	private void WriteData(Stream archiveStream, Stream dataStream)
	{
		SetDataOffset(this, archiveStream);
		dataStream.CopyTo(archiveStream);
		WriteEmptyPadding(archiveStream);
	}

	private void WriteEmptyPadding(Stream archiveStream)
	{
		int num = TarHelpers.CalculatePadding(_size);
		if (num != 0)
		{
			Span<byte> span = stackalloc byte[512];
			span = span.Slice(0, num);
			span.Clear();
			archiveStream.Write(span);
		}
	}

	private ValueTask WriteEmptyPaddingAsync(Stream archiveStream, CancellationToken cancellationToken)
	{
		int num = TarHelpers.CalculatePadding(_size);
		if (num != 0)
		{
			byte[] array = new byte[num];
			return archiveStream.WriteAsync(array, cancellationToken);
		}
		return ValueTask.CompletedTask;
	}

	private async Task WriteDataAsync(Stream archiveStream, Stream dataStream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		SetDataOffset(this, archiveStream);
		await dataStream.CopyToAsync(archiveStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		int num = TarHelpers.CalculatePadding(_size);
		if (num != 0)
		{
			byte[] buffer = ArrayPool<byte>.Shared.Rent(num);
			Array.Clear(buffer, 0, num);
			await archiveStream.WriteAsync(buffer.AsMemory(0, num), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	private static MemoryStream GenerateExtendedAttributesDataStream(Dictionary<string, string> extendedAttributes)
	{
		MemoryStream memoryStream = null;
		byte[] array = null;
		Span<byte> destination = stackalloc byte[512];
		if (extendedAttributes.Count > 0)
		{
			memoryStream = new MemoryStream();
			foreach (var (text3, text4) in extendedAttributes)
			{
				int num = 3 + Encoding.UTF8.GetByteCount(text3) + Encoding.UTF8.GetByteCount(text4);
				int num2 = CountDigits(num);
				num += num2;
				int num3;
				while ((num3 = CountDigits(num)) != num2)
				{
					num += num3 - num2;
					num2 = num3;
				}
				if (destination.Length < num)
				{
					if (array != null)
					{
						ArrayPool<byte>.Shared.Return(array);
					}
					destination = (array = ArrayPool<byte>.Shared.Rent(num));
				}
				Utf8Formatter.TryFormat(num, destination, out var bytesWritten);
				destination[bytesWritten++] = 32;
				bytesWritten += Encoding.UTF8.GetBytes(text3.AsSpan(), destination.Slice(bytesWritten));
				destination[bytesWritten++] = 61;
				bytesWritten += Encoding.UTF8.GetBytes(text4.AsSpan(), destination.Slice(bytesWritten));
				destination[bytesWritten++] = 10;
				memoryStream.Write(destination.Slice(0, bytesWritten));
			}
			memoryStream.Position = 0L;
		}
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		return memoryStream;
		static int CountDigits(int value)
		{
			int num4 = 1;
			while (true)
			{
				value /= 10;
				if (value == 0)
				{
					break;
				}
				num4++;
			}
			return num4;
		}
	}

	private void CollectExtendedAttributesFromStandardFieldsIfNeeded()
	{
		ExtendedAttributes["path"] = _name;
		ExtendedAttributes["mtime"] = TarHelpers.GetTimestampStringFromDateTimeOffset(_mTime);
		TryAddStringField(ExtendedAttributes, "gname", _gName, 32);
		TryAddStringField(ExtendedAttributes, "uname", _uName, 32);
		if (!string.IsNullOrEmpty(_linkName))
		{
			ExtendedAttributes["linkpath"] = _linkName;
		}
		if (_size > 8589934591L)
		{
			ExtendedAttributes["size"] = _size.ToString();
		}
		else
		{
			ExtendedAttributes.Remove("size");
		}
		if (_uid > 2097151)
		{
			ExtendedAttributes["uid"] = _uid.ToString();
		}
		else
		{
			ExtendedAttributes.Remove("uid");
		}
		if (_gid > 2097151)
		{
			ExtendedAttributes["gid"] = _gid.ToString();
		}
		else
		{
			ExtendedAttributes.Remove("gid");
		}
		if (_devMajor > 2097151)
		{
			ExtendedAttributes["devmajor"] = _devMajor.ToString();
		}
		else
		{
			ExtendedAttributes.Remove("devmajor");
		}
		if (_devMinor > 2097151)
		{
			ExtendedAttributes["devminor"] = _devMinor.ToString();
		}
		else
		{
			ExtendedAttributes.Remove("devminor");
		}
		static void TryAddStringField(Dictionary<string, string> extendedAttributes, string key, string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value) || GetUtf8TextLength(value.AsSpan()) <= maxLength)
			{
				extendedAttributes.Remove(key);
			}
			else
			{
				extendedAttributes[key] = value;
			}
		}
	}

	private static int WriteChecksum(int checksum, Span<byte> buffer)
	{
		checksum += 256;
		Span<byte> destination = stackalloc byte[8];
		destination.Clear();
		FormatOctal(checksum, destination);
		Span<byte> span = buffer.Slice(148, 8);
		span[span.Length - 1] = 32;
		span[span.Length - 2] = 0;
		int num = span.Length - 3;
		int num2 = destination.Length - 2;
		while (num >= 0)
		{
			if (num2 >= 0)
			{
				span[num] = destination[num2];
				num2--;
			}
			else
			{
				span[num] = 48;
			}
			num--;
		}
		return checksum;
	}

	private static int WriteLeftAlignedBytesAndGetChecksum(ReadOnlySpan<byte> bytesToWrite, Span<byte> destination)
	{
		bytesToWrite = bytesToWrite[..Math.Min(bytesToWrite.Length, destination.Length)];
		bytesToWrite.CopyTo(destination);
		return Checksum(bytesToWrite);
	}

	private static int WriteRightAlignedBytesAndGetChecksum(ReadOnlySpan<byte> bytesToWrite, Span<byte> destination)
	{
		destination[destination.Length - 1] = 0;
		bytesToWrite = bytesToWrite[..Math.Min(bytesToWrite.Length, destination.Length - 1)];
		int num = destination.Length - 1 - bytesToWrite.Length;
		bytesToWrite.CopyTo(destination.Slice(num));
		destination.Slice(0, num).Fill(48);
		return Checksum(destination);
	}

	private static int Checksum(ReadOnlySpan<byte> bytes)
	{
		int num = 0;
		ReadOnlySpan<byte> readOnlySpan = bytes;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			num += b;
		}
		return num;
	}

	private int FormatNumeric(int value, Span<byte> destination)
	{
		if ((value >= 0 && value <= 2097151) || _format == TarEntryFormat.Pax)
		{
			return FormatOctal(value, destination);
		}
		if (_format == TarEntryFormat.Gnu)
		{
			long num = value;
			num |= long.MinValue;
			BinaryPrimitives.WriteInt64BigEndian(destination, num);
			return Checksum(destination);
		}
		throw new ArgumentException(System.SR.Format(System.SR.TarFieldTooLargeForEntryFormat, _format));
	}

	private int FormatNumeric(long value, Span<byte> destination)
	{
		if ((value >= 0 && value <= 8589934591L) || _format == TarEntryFormat.Pax)
		{
			return FormatOctal(value, destination);
		}
		if (_format == TarEntryFormat.Gnu)
		{
			BinaryPrimitives.WriteUInt32BigEndian(destination, (value < 0) ? uint.MaxValue : 2147483648u);
			BinaryPrimitives.WriteInt64BigEndian(destination.Slice(4), value);
			return Checksum(destination);
		}
		throw new ArgumentException(System.SR.Format(System.SR.TarFieldTooLargeForEntryFormat, _format));
	}

	private static int FormatOctal(long value, Span<byte> destination)
	{
		ulong num = (ulong)value;
		Span<byte> span = stackalloc byte[32];
		int num2 = span.Length - 1;
		while (true)
		{
			span[num2] = (byte)(48 + num % 8);
			num /= 8;
			if (num == 0L)
			{
				break;
			}
			num2--;
		}
		return WriteRightAlignedBytesAndGetChecksum(span.Slice(num2), destination);
	}

	private int WriteAsTimestamp(DateTimeOffset timestamp, Span<byte> destination)
	{
		if (timestamp == default(DateTimeOffset))
		{
			return 0;
		}
		long value = timestamp.ToUnixTimeSeconds();
		return FormatNumeric(value, destination);
	}

	private static int WriteAsUtf8String(ReadOnlySpan<char> text, Span<byte> buffer)
	{
		return WriteLeftAlignedBytesAndGetChecksum(buffer[..Encoding.UTF8.GetBytes(text, buffer)], buffer);
	}

	private string GenerateExtendedAttributeName()
	{
		ReadOnlySpan<char> directoryName = Path.GetDirectoryName(_name.AsSpan());
		directoryName = (directoryName.IsEmpty ? ".".AsSpan() : directoryName);
		ReadOnlySpan<char> fileName = Path.GetFileName(_name.AsSpan());
		fileName = (fileName.IsEmpty ? ".".AsSpan() : fileName);
		TarEntryType typeFlag = _typeFlag;
		if ((typeFlag == TarEntryType.Directory || typeFlag == TarEntryType.DirectoryList) ? true : false)
		{
			return $"{directoryName}/PaxHeaders.{Environment.ProcessId}/{fileName}{Path.DirectorySeparatorChar}";
		}
		return $"{directoryName}/PaxHeaders.{Environment.ProcessId}/{fileName}";
	}

	private static string GenerateGlobalExtendedAttributeName(int globalExtendedAttributesEntryNumber)
	{
		ReadOnlySpan<char> value = Path.TrimEndingDirectorySeparator(Path.GetTempPath()).AsSpan();
		string text = $"{value}/GlobalHead.{Environment.ProcessId}.{globalExtendedAttributesEntryNumber}";
		if (text.Length < 100)
		{
			return text;
		}
		return string.Concat("/tmp".AsSpan(), text.AsSpan(value.Length));
	}

	private static int GetUtf8TextLength(ReadOnlySpan<char> text)
	{
		return Encoding.UTF8.GetByteCount(text);
	}

	private static int GetUtf16TruncatedTextLength(ReadOnlySpan<char> text, int utf8MaxLength)
	{
		int num = 0;
		int num2 = 0;
		SpanRuneEnumerator enumerator = text.EnumerateRunes().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Rune current = enumerator.Current;
				num += current.Utf8SequenceLength;
				if (num <= utf8MaxLength)
				{
					num2 += current.Utf16SequenceLength;
					continue;
				}
				break;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return num2;
	}
}
