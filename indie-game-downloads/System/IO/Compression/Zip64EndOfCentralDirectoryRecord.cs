using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class Zip64EndOfCentralDirectoryRecord
{
	public ulong SizeOfThisRecord;

	public ushort VersionMadeBy;

	public ushort VersionNeededToExtract;

	public uint NumberOfThisDisk;

	public uint NumberOfDiskWithStartOfCD;

	public ulong NumberOfEntriesOnThisDisk;

	public ulong NumberOfEntriesTotal;

	public ulong SizeOfCentralDirectory;

	public ulong OffsetOfCentralDirectory;

	public static ReadOnlySpan<byte> SignatureConstantBytes => new byte[4] { 80, 75, 6, 6 };

	private static bool TryReadBlockCore(Span<byte> blockContents, int bytesRead, [NotNullWhen(true)] out Zip64EndOfCentralDirectoryRecord zip64EOCDRecord)
	{
		zip64EOCDRecord = null;
		if (bytesRead < 56)
		{
			return false;
		}
		if (!((ReadOnlySpan<byte>)blockContents).StartsWith(SignatureConstantBytes))
		{
			return false;
		}
		Zip64EndOfCentralDirectoryRecord zip64EndOfCentralDirectoryRecord = new Zip64EndOfCentralDirectoryRecord();
		zip64EndOfCentralDirectoryRecord.SizeOfThisRecord = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(4, blockContents.Length - 4));
		zip64EndOfCentralDirectoryRecord.VersionMadeBy = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(12, blockContents.Length - 12));
		zip64EndOfCentralDirectoryRecord.VersionNeededToExtract = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(14, blockContents.Length - 14));
		zip64EndOfCentralDirectoryRecord.NumberOfThisDisk = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16));
		zip64EndOfCentralDirectoryRecord.NumberOfDiskWithStartOfCD = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(20, blockContents.Length - 20));
		zip64EndOfCentralDirectoryRecord.NumberOfEntriesOnThisDisk = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(24, blockContents.Length - 24));
		zip64EndOfCentralDirectoryRecord.NumberOfEntriesTotal = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(32, blockContents.Length - 32));
		zip64EndOfCentralDirectoryRecord.SizeOfCentralDirectory = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(40, blockContents.Length - 40));
		zip64EndOfCentralDirectoryRecord.OffsetOfCentralDirectory = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(48, blockContents.Length - 48));
		zip64EOCDRecord = zip64EndOfCentralDirectoryRecord;
		return true;
	}

	public static Zip64EndOfCentralDirectoryRecord TryReadBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[56];
		int bytesRead = stream.ReadAtLeast(span, span.Length, throwOnEndOfStream: false);
		if (!TryReadBlockCore(span, bytesRead, out var zip64EOCDRecord))
		{
			throw new InvalidDataException(System.SR.Zip64EOCDNotWhereExpected);
		}
		return zip64EOCDRecord;
	}

	private static void WriteBlockCore(Span<byte> blockContents, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory)
	{
		ReadOnlySpan<byte> signatureConstantBytes = SignatureConstantBytes;
		signatureConstantBytes.CopyTo(blockContents.Slice(0, blockContents.Length));
		BinaryPrimitives.WriteUInt64LittleEndian(blockContents.Slice(4, blockContents.Length - 4), 44uL);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(12, blockContents.Length - 12), 45);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(14, blockContents.Length - 14), 45);
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16), 0u);
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(20, blockContents.Length - 20), 0u);
		BinaryPrimitives.WriteInt64LittleEndian(blockContents.Slice(24, blockContents.Length - 24), numberOfEntries);
		BinaryPrimitives.WriteInt64LittleEndian(blockContents.Slice(32, blockContents.Length - 32), numberOfEntries);
		BinaryPrimitives.WriteInt64LittleEndian(blockContents.Slice(40, blockContents.Length - 40), sizeOfCentralDirectory);
		BinaryPrimitives.WriteInt64LittleEndian(blockContents.Slice(48, blockContents.Length - 48), startOfCentralDirectory);
	}

	public static void WriteBlock(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory)
	{
		Span<byte> span = stackalloc byte[56];
		WriteBlockCore(span, numberOfEntries, startOfCentralDirectory, sizeOfCentralDirectory);
		stream.Write(span);
	}

	public static async Task<Zip64EndOfCentralDirectoryRecord> TryReadBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] blockContents = new byte[56];
		int bytesRead = await stream.ReadAtLeastAsync(blockContents, blockContents.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!TryReadBlockCore(blockContents, bytesRead, out var zip64EOCDRecord))
		{
			throw new InvalidDataException(System.SR.Zip64EOCDNotWhereExpected);
		}
		return zip64EOCDRecord;
	}

	public static ValueTask WriteBlockAsync(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[56];
		WriteBlockCore(array, numberOfEntries, startOfCentralDirectory, sizeOfCentralDirectory);
		return stream.WriteAsync(array, cancellationToken);
	}
}
