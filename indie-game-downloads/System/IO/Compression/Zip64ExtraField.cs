using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class Zip64ExtraField
{
	private ushort _size;

	private long? _uncompressedSize;

	private long? _compressedSize;

	private long? _localHeaderOffset;

	private uint? _startDiskNumber;

	public ushort TotalSize => (ushort)(_size + 4);

	public long? UncompressedSize
	{
		get
		{
			return _uncompressedSize;
		}
		set
		{
			_uncompressedSize = value;
			UpdateSize();
		}
	}

	public long? CompressedSize
	{
		get
		{
			return _compressedSize;
		}
		set
		{
			_compressedSize = value;
			UpdateSize();
		}
	}

	public long? LocalHeaderOffset
	{
		get
		{
			return _localHeaderOffset;
		}
		set
		{
			_localHeaderOffset = value;
			UpdateSize();
		}
	}

	public uint? StartDiskNumber => _startDiskNumber;

	private void UpdateSize()
	{
		_size = 0;
		if (_uncompressedSize.HasValue)
		{
			_size += 8;
		}
		if (_compressedSize.HasValue)
		{
			_size += 8;
		}
		if (_localHeaderOffset.HasValue)
		{
			_size += 8;
		}
		if (_startDiskNumber.HasValue)
		{
			_size += 4;
		}
	}

	public static Zip64ExtraField GetJustZip64Block(ReadOnlySpan<byte> extraFieldData, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber)
	{
		int num = 0;
		int bytesConsumed;
		ZipGenericExtraField field;
		while (ZipGenericExtraField.TryReadBlock(extraFieldData.Slice(num), out bytesConsumed, out field))
		{
			num += bytesConsumed;
			if (TryGetZip64BlockFromGenericExtraField(field, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber, out var zip64Block))
			{
				return zip64Block;
			}
		}
		return new Zip64ExtraField
		{
			_compressedSize = null,
			_uncompressedSize = null,
			_localHeaderOffset = null,
			_startDiskNumber = null
		};
	}

	private static bool TryGetZip64BlockFromGenericExtraField(ZipGenericExtraField extraField, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber, out Zip64ExtraField zip64Block)
	{
		zip64Block = new Zip64ExtraField
		{
			_compressedSize = null,
			_uncompressedSize = null,
			_localHeaderOffset = null,
			_startDiskNumber = null
		};
		if (extraField.Tag != 1)
		{
			return false;
		}
		zip64Block._size = extraField.Size;
		ReadOnlySpan<byte> source = extraField.Data;
		if (source.Length < 8)
		{
			return true;
		}
		bool flag = extraField.Size >= 28;
		if (readUncompressedSize)
		{
			zip64Block._uncompressedSize = BinaryPrimitives.ReadInt64LittleEndian(source);
			source = source.Slice(8);
		}
		else if (flag)
		{
			source = source.Slice(8);
		}
		if (source.Length < 8)
		{
			return true;
		}
		if (readCompressedSize)
		{
			zip64Block._compressedSize = BinaryPrimitives.ReadInt64LittleEndian(source);
			source = source.Slice(8);
		}
		else if (flag)
		{
			source = source.Slice(8);
		}
		if (source.Length < 8)
		{
			return true;
		}
		if (readLocalHeaderOffset)
		{
			zip64Block._localHeaderOffset = BinaryPrimitives.ReadInt64LittleEndian(source);
			source = source.Slice(8);
		}
		else if (flag)
		{
			source = source.Slice(8);
		}
		if (source.Length < 4)
		{
			return true;
		}
		if (readStartDiskNumber)
		{
			zip64Block._startDiskNumber = BinaryPrimitives.ReadUInt32LittleEndian(source);
		}
		if (zip64Block._uncompressedSize < 0)
		{
			throw new InvalidDataException(System.SR.FieldTooBigUncompressedSize);
		}
		if (zip64Block._compressedSize < 0)
		{
			throw new InvalidDataException(System.SR.FieldTooBigCompressedSize);
		}
		if (zip64Block._localHeaderOffset < 0)
		{
			throw new InvalidDataException(System.SR.FieldTooBigLocalHeaderOffset);
		}
		return true;
	}

	public static Zip64ExtraField GetAndRemoveZip64Block(List<ZipGenericExtraField> extraFields, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber)
	{
		Zip64ExtraField zip64Field = new Zip64ExtraField
		{
			_compressedSize = null,
			_uncompressedSize = null,
			_localHeaderOffset = null,
			_startDiskNumber = null
		};
		bool zip64FieldFound = false;
		extraFields.RemoveAll(delegate(ZipGenericExtraField ef)
		{
			if (ef.Tag == 1)
			{
				if (!zip64FieldFound && TryGetZip64BlockFromGenericExtraField(ef, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber, out zip64Field))
				{
					zip64FieldFound = true;
				}
				return true;
			}
			return false;
		});
		return zip64Field;
	}

	public static void RemoveZip64Blocks(List<ZipGenericExtraField> extraFields)
	{
		extraFields.RemoveAll((ZipGenericExtraField field) => field.Tag == 1);
	}

	public void WriteBlockCore(Span<byte> extraFieldData)
	{
		int num = 4;
		BinaryPrimitives.WriteUInt16LittleEndian(extraFieldData.Slice(0, extraFieldData.Length), 1);
		BinaryPrimitives.WriteUInt16LittleEndian(extraFieldData.Slice(2, extraFieldData.Length - 2), _size);
		if (_uncompressedSize.HasValue)
		{
			int num2 = num;
			BinaryPrimitives.WriteInt64LittleEndian(extraFieldData.Slice(num2, extraFieldData.Length - num2), _uncompressedSize.Value);
			num += 8;
		}
		if (_compressedSize.HasValue)
		{
			int num2 = num;
			BinaryPrimitives.WriteInt64LittleEndian(extraFieldData.Slice(num2, extraFieldData.Length - num2), _compressedSize.Value);
			num += 8;
		}
		if (_localHeaderOffset.HasValue)
		{
			int num2 = num;
			BinaryPrimitives.WriteInt64LittleEndian(extraFieldData.Slice(num2, extraFieldData.Length - num2), _localHeaderOffset.Value);
			num += 8;
		}
		if (_startDiskNumber.HasValue)
		{
			int num2 = num;
			BinaryPrimitives.WriteUInt32LittleEndian(extraFieldData.Slice(num2, extraFieldData.Length - num2), _startDiskNumber.Value);
		}
	}

	public void WriteBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[(int)TotalSize];
		WriteBlockCore(span);
		stream.Write(span);
	}

	public ValueTask WriteBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[TotalSize];
		WriteBlockCore(array);
		return stream.WriteAsync(array, cancellationToken);
	}
}
