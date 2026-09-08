using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class ZipGenericExtraField
{
	private ushort _tag;

	private ushort _size;

	private byte[] _data;

	public ushort Tag => _tag;

	public ushort Size => _size;

	public byte[] Data => _data ?? (_data = Array.Empty<byte>());

	public void WriteBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[4];
		WriteBlockCore(span);
		stream.Write(span);
		stream.Write(Data);
	}

	private void WriteBlockCore(Span<byte> extraFieldHeader)
	{
		BinaryPrimitives.WriteUInt16LittleEndian(extraFieldHeader.Slice(0, extraFieldHeader.Length), _tag);
		BinaryPrimitives.WriteUInt16LittleEndian(extraFieldHeader.Slice(2, extraFieldHeader.Length - 2), _size);
	}

	public static bool TryReadBlock(ReadOnlySpan<byte> bytes, out int bytesConsumed, out ZipGenericExtraField field)
	{
		field = new ZipGenericExtraField();
		bytesConsumed = 0;
		if (bytes.Length < 4)
		{
			return false;
		}
		field._tag = BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(0, bytes.Length));
		field._size = BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(2, bytes.Length - 2));
		if (bytes.Length - 4 < field._size)
		{
			return false;
		}
		field._data = bytes.Slice(4, field._size).ToArray();
		bytesConsumed = field.Size + 4;
		return true;
	}

	public static List<ZipGenericExtraField> ParseExtraField(ReadOnlySpan<byte> extraFieldData, out ReadOnlySpan<byte> trailingExtraFieldData)
	{
		List<ZipGenericExtraField> list = new List<ZipGenericExtraField>();
		int num = 0;
		int num2;
		while (true)
		{
			num2 = num;
			if (!TryReadBlock(extraFieldData.Slice(num2, extraFieldData.Length - num2), out var bytesConsumed, out var field))
			{
				break;
			}
			num += bytesConsumed;
			list.Add(field);
		}
		num2 = num;
		trailingExtraFieldData = extraFieldData.Slice(num2, extraFieldData.Length - num2);
		return list;
	}

	public static int TotalSize(List<ZipGenericExtraField> fields, int trailingDataLength)
	{
		int num = trailingDataLength;
		if (fields != null)
		{
			foreach (ZipGenericExtraField field in fields)
			{
				num += field.Size + 4;
			}
		}
		return num;
	}

	public static void WriteAllBlocks(List<ZipGenericExtraField> fields, ReadOnlySpan<byte> trailingExtraFieldData, Stream stream)
	{
		if (fields != null)
		{
			foreach (ZipGenericExtraField field in fields)
			{
				field.WriteBlock(stream);
			}
		}
		if (!trailingExtraFieldData.IsEmpty)
		{
			stream.Write(trailingExtraFieldData);
		}
	}

	public async Task WriteBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[4];
		WriteBlockCore(array);
		await stream.WriteAsync(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await stream.WriteAsync(Data, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public static async Task WriteAllBlocksAsync(List<ZipGenericExtraField> fields, ReadOnlyMemory<byte> trailingExtraFieldData, Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (fields != null)
		{
			foreach (ZipGenericExtraField field in fields)
			{
				await field.WriteBlockAsync(stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		if (!trailingExtraFieldData.IsEmpty)
		{
			await stream.WriteAsync(trailingExtraFieldData, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
