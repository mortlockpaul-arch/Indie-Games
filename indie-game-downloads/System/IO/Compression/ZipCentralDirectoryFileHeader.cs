using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class ZipCentralDirectoryFileHeader
{
	public byte VersionMadeByCompatibility;

	public byte VersionMadeBySpecification;

	public ushort VersionNeededToExtract;

	public ushort GeneralPurposeBitFlag;

	public ushort CompressionMethod;

	public uint LastModified;

	public uint Crc32;

	public long CompressedSize;

	public long UncompressedSize;

	public ushort FilenameLength;

	public ushort ExtraFieldLength;

	public ushort FileCommentLength;

	public uint DiskNumberStart;

	public ushort InternalFileAttributes;

	public uint ExternalFileAttributes;

	public long RelativeOffsetOfLocalHeader;

	public byte[] Filename = Array.Empty<byte>();

	public byte[] FileComment = Array.Empty<byte>();

	public List<ZipGenericExtraField> ExtraFields;

	public byte[] TrailingExtraFieldData;

	public static ReadOnlySpan<byte> SignatureConstantBytes => new byte[4] { 80, 75, 1, 2 };

	private static bool TryReadBlockInitialize(ReadOnlySpan<byte> buffer, [NotNullWhen(true)] out ZipCentralDirectoryFileHeader header, out int bytesRead, out uint compressedSizeSmall, out uint uncompressedSizeSmall, out ushort diskNumberStartSmall, out uint relativeOffsetOfLocalHeaderSmall)
	{
		header = null;
		bytesRead = 0;
		compressedSizeSmall = 0u;
		uncompressedSizeSmall = 0u;
		diskNumberStartSmall = 0;
		relativeOffsetOfLocalHeaderSmall = 0u;
		if (!buffer.StartsWith(SignatureConstantBytes))
		{
			return false;
		}
		ZipCentralDirectoryFileHeader obj = new ZipCentralDirectoryFileHeader
		{
			VersionMadeBySpecification = buffer[4],
			VersionMadeByCompatibility = buffer[5]
		};
		obj.VersionNeededToExtract = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(6, buffer.Length - 6));
		obj.GeneralPurposeBitFlag = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(8, buffer.Length - 8));
		obj.CompressionMethod = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(10, buffer.Length - 10));
		obj.LastModified = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(12, buffer.Length - 12));
		obj.Crc32 = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(16, buffer.Length - 16));
		obj.FilenameLength = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(28, buffer.Length - 28));
		obj.ExtraFieldLength = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(30, buffer.Length - 30));
		obj.FileCommentLength = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(32, buffer.Length - 32));
		obj.InternalFileAttributes = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(36, buffer.Length - 36));
		obj.ExternalFileAttributes = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(38, buffer.Length - 38));
		header = obj;
		compressedSizeSmall = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(20, buffer.Length - 20));
		uncompressedSizeSmall = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(24, buffer.Length - 24));
		diskNumberStartSmall = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(34, buffer.Length - 34));
		relativeOffsetOfLocalHeaderSmall = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(42, buffer.Length - 42));
		return true;
	}

	private static void TryReadBlockFinalize(ZipCentralDirectoryFileHeader header, ReadOnlySpan<byte> dynamicHeader, int dynamicHeaderSize, uint uncompressedSizeSmall, uint compressedSizeSmall, ushort diskNumberStartSmall, uint relativeOffsetOfLocalHeaderSmall, bool saveExtraFieldsAndComments, ref int bytesRead, out Zip64ExtraField zip64)
	{
		header.Filename = dynamicHeader.Slice(0, header.FilenameLength).ToArray();
		bool readUncompressedSize = uncompressedSizeSmall == uint.MaxValue;
		bool readCompressedSize = compressedSizeSmall == uint.MaxValue;
		bool readLocalHeaderOffset = relativeOffsetOfLocalHeaderSmall == uint.MaxValue;
		bool readStartDiskNumber = diskNumberStartSmall == ushort.MaxValue;
		ReadOnlySpan<byte> extraFieldData = dynamicHeader.Slice(header.FilenameLength, header.ExtraFieldLength);
		if (saveExtraFieldsAndComments)
		{
			header.ExtraFields = ZipGenericExtraField.ParseExtraField(extraFieldData, out var trailingExtraFieldData);
			zip64 = Zip64ExtraField.GetAndRemoveZip64Block(header.ExtraFields, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber);
			header.TrailingExtraFieldData = trailingExtraFieldData.ToArray();
		}
		else
		{
			header.ExtraFields = null;
			header.TrailingExtraFieldData = null;
			zip64 = Zip64ExtraField.GetJustZip64Block(extraFieldData, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber);
		}
		header.FileComment = dynamicHeader.Slice(header.FilenameLength + header.ExtraFieldLength, header.FileCommentLength).ToArray();
		bytesRead = 46 + dynamicHeaderSize;
		header.UncompressedSize = zip64.UncompressedSize ?? uncompressedSizeSmall;
		header.CompressedSize = zip64.CompressedSize ?? compressedSizeSmall;
		header.RelativeOffsetOfLocalHeader = zip64.LocalHeaderOffset ?? relativeOffsetOfLocalHeaderSmall;
		header.DiskNumberStart = zip64.StartDiskNumber ?? diskNumberStartSmall;
	}

	public static bool TryReadBlock(ReadOnlySpan<byte> buffer, Stream furtherReads, bool saveExtraFieldsAndComments, out int bytesRead, [NotNullWhen(true)] out ZipCentralDirectoryFileHeader header)
	{
		if (!TryReadBlockInitialize(buffer, out header, out bytesRead, out var compressedSizeSmall, out var uncompressedSizeSmall, out var diskNumberStartSmall, out var relativeOffsetOfLocalHeaderSmall))
		{
			return false;
		}
		byte[] array = null;
		try
		{
			int num = header.FilenameLength + header.ExtraFieldLength + header.FileCommentLength;
			int num2 = buffer.Length - 46;
			int num3 = num - num2;
			ReadOnlySpan<byte> dynamicHeader;
			if (num3 <= 0)
			{
				dynamicHeader = buffer.Slice(46, buffer.Length - 46);
			}
			else
			{
				if (num > 512)
				{
					array = ArrayPool<byte>.Shared.Rent(num);
				}
				Span<byte> span = ((num > 512) ? array.AsSpan(0, num) : stackalloc byte[512].Slice(0, num));
				Span<byte> span2 = span;
				buffer.Slice(46, buffer.Length - 46).CopyTo(span2);
				int num4 = num2;
				if (furtherReads.ReadAtLeast(span2.Slice(num4, span2.Length - num4), num3, throwOnEndOfStream: false) != num3)
				{
					return false;
				}
				dynamicHeader = span2;
			}
			TryReadBlockFinalize(header, dynamicHeader, num, uncompressedSizeSmall, compressedSizeSmall, diskNumberStartSmall, relativeOffsetOfLocalHeaderSmall, saveExtraFieldsAndComments, ref bytesRead, out var _);
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		return true;
	}

	public static async Task<(bool, int, ZipCentralDirectoryFileHeader)> TryReadBlockAsync(ReadOnlyMemory<byte> buffer, Stream furtherReads, bool saveExtraFieldsAndComments, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (!TryReadBlockInitialize(buffer.Span, out var header, out var bytesRead, out var compressedSizeSmall, out var uncompressedSizeSmall, out var diskNumberStartSmall, out var relativeOffsetOfLocalHeaderSmall))
		{
			return (false, 0, null);
		}
		byte[] arrayPoolBuffer = null;
		try
		{
			int dynamicHeaderSize = header.FilenameLength + header.ExtraFieldLength + header.FileCommentLength;
			int num = buffer.Length - 46;
			int bytesToRead = dynamicHeaderSize - num;
			ReadOnlySpan<byte> dynamicHeader;
			if (bytesToRead <= 0)
			{
				ReadOnlySpan<byte> span = buffer.Span;
				dynamicHeader = span.Slice(46, span.Length - 46);
			}
			else
			{
				if (dynamicHeaderSize > 512)
				{
					arrayPoolBuffer = ArrayPool<byte>.Shared.Rent(dynamicHeaderSize);
				}
				byte[] collatedHeader = ((dynamicHeaderSize <= 512) ? new byte[dynamicHeaderSize] : arrayPoolBuffer.AsSpan(0, dynamicHeaderSize).ToArray());
				buffer.Slice(46, buffer.Length - 46).CopyTo(collatedHeader);
				if (await furtherReads.ReadAtLeastAsync(collatedHeader.AsMemory(num..), bytesToRead, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != bytesToRead)
				{
					return (false, bytesRead, null);
				}
				dynamicHeader = collatedHeader;
			}
			TryReadBlockFinalize(header, dynamicHeader, dynamicHeaderSize, uncompressedSizeSmall, compressedSizeSmall, diskNumberStartSmall, relativeOffsetOfLocalHeaderSmall, saveExtraFieldsAndComments, ref bytesRead, out var _);
		}
		finally
		{
			if (arrayPoolBuffer != null)
			{
				ArrayPool<byte>.Shared.Return(arrayPoolBuffer);
			}
		}
		return (true, bytesRead, header);
	}
}
