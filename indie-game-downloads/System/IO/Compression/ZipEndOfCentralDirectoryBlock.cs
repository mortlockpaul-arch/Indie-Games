using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class ZipEndOfCentralDirectoryBlock
{
	public static readonly byte[] SignatureConstantBytes = new byte[4] { 80, 75, 5, 6 };

	public uint Signature;

	public ushort NumberOfThisDisk;

	public ushort NumberOfTheDiskWithTheStartOfTheCentralDirectory;

	public ushort NumberOfEntriesInTheCentralDirectoryOnThisDisk;

	public ushort NumberOfEntriesInTheCentralDirectory;

	public uint SizeOfCentralDirectory;

	public uint OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;

	private byte[] _archiveComment;

	public byte[] ArchiveComment => _archiveComment ?? (_archiveComment = Array.Empty<byte>());

	private static void WriteBlockInitialize(Span<byte> blockContents, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory, byte[] archiveComment)
	{
		ushort value = ((numberOfEntries > 65535) ? ushort.MaxValue : ((ushort)numberOfEntries));
		uint value2 = (uint)((startOfCentralDirectory > uint.MaxValue) ? uint.MaxValue : startOfCentralDirectory);
		uint value3 = (uint)((sizeOfCentralDirectory > uint.MaxValue) ? uint.MaxValue : sizeOfCentralDirectory);
		SignatureConstantBytes.CopyTo(blockContents.Slice(0, blockContents.Length));
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(4, blockContents.Length - 4), 0);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(6, blockContents.Length - 6), 0);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(8, blockContents.Length - 8), value);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(10, blockContents.Length - 10), value);
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(12, blockContents.Length - 12), value3);
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16), value2);
		BinaryPrimitives.WriteUInt16LittleEndian(blockContents.Slice(20, blockContents.Length - 20), (ushort)archiveComment.Length);
	}

	public static void WriteBlock(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory, byte[] archiveComment)
	{
		Span<byte> span = stackalloc byte[22];
		WriteBlockInitialize(span, numberOfEntries, startOfCentralDirectory, sizeOfCentralDirectory, archiveComment);
		stream.Write(span);
		if (archiveComment.Length != 0)
		{
			stream.Write(archiveComment);
		}
	}

	private static bool TryReadBlockInitialize(Stream stream, Span<byte> blockContents, int bytesRead, [NotNullWhen(true)] out ZipEndOfCentralDirectoryBlock eocdBlock, out bool readComment)
	{
		readComment = false;
		eocdBlock = null;
		if (bytesRead < 22)
		{
			return false;
		}
		if (!((ReadOnlySpan<byte>)blockContents).StartsWith((ReadOnlySpan<byte>)SignatureConstantBytes))
		{
			return false;
		}
		ZipEndOfCentralDirectoryBlock zipEndOfCentralDirectoryBlock = new ZipEndOfCentralDirectoryBlock();
		zipEndOfCentralDirectoryBlock.Signature = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(0, blockContents.Length));
		zipEndOfCentralDirectoryBlock.NumberOfThisDisk = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(4, blockContents.Length - 4));
		zipEndOfCentralDirectoryBlock.NumberOfTheDiskWithTheStartOfTheCentralDirectory = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(6, blockContents.Length - 6));
		zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectoryOnThisDisk = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(8, blockContents.Length - 8));
		zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectory = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(10, blockContents.Length - 10));
		zipEndOfCentralDirectoryBlock.SizeOfCentralDirectory = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(12, blockContents.Length - 12));
		zipEndOfCentralDirectoryBlock.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16));
		eocdBlock = zipEndOfCentralDirectoryBlock;
		ushort num = BinaryPrimitives.ReadUInt16LittleEndian(blockContents.Slice(20, blockContents.Length - 20));
		if (stream.Position + num > stream.Length)
		{
			return false;
		}
		if (num == 0)
		{
			eocdBlock._archiveComment = Array.Empty<byte>();
		}
		else
		{
			eocdBlock._archiveComment = new byte[num];
			readComment = true;
		}
		return true;
	}

	public static ZipEndOfCentralDirectoryBlock ReadBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[22];
		int bytesRead = stream.ReadAtLeast(span, span.Length, throwOnEndOfStream: false);
		if (!TryReadBlockInitialize(stream, span, bytesRead, out var eocdBlock, out var readComment))
		{
			throw new InvalidDataException(System.SR.EOCDNotFound);
		}
		if (readComment)
		{
			stream.ReadExactly(eocdBlock._archiveComment);
		}
		return eocdBlock;
	}

	public static async Task WriteBlockAsync(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory, byte[] archiveComment, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[22];
		WriteBlockInitialize(array, numberOfEntries, startOfCentralDirectory, sizeOfCentralDirectory, archiveComment);
		await stream.WriteAsync(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (archiveComment.Length != 0)
		{
			await stream.WriteAsync(archiveComment, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public static async Task<ZipEndOfCentralDirectoryBlock> ReadBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] blockContents = new byte[22];
		int bytesRead = await stream.ReadAtLeastAsync(blockContents, blockContents.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!TryReadBlockInitialize(stream, blockContents, bytesRead, out var eocdBlock, out var readComment))
		{
			throw new InvalidDataException(System.SR.EOCDNotFound);
		}
		if (readComment)
		{
			stream.ReadExactly(eocdBlock._archiveComment);
		}
		return eocdBlock;
	}
}
