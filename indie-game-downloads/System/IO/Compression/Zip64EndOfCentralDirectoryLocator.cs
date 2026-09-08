using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

internal sealed class Zip64EndOfCentralDirectoryLocator
{
	public static readonly byte[] SignatureConstantBytes = new byte[4] { 80, 75, 6, 7 };

	public uint NumberOfDiskWithZip64EOCD;

	public ulong OffsetOfZip64EOCD;

	public uint TotalNumberOfDisks;

	private static bool TryReadBlockCore(Span<byte> blockContents, int bytesRead, [NotNullWhen(true)] out Zip64EndOfCentralDirectoryLocator zip64EOCDLocator)
	{
		zip64EOCDLocator = null;
		if (bytesRead < 20 || !((ReadOnlySpan<byte>)blockContents).StartsWith((ReadOnlySpan<byte>)SignatureConstantBytes))
		{
			return false;
		}
		Zip64EndOfCentralDirectoryLocator zip64EndOfCentralDirectoryLocator = new Zip64EndOfCentralDirectoryLocator();
		zip64EndOfCentralDirectoryLocator.NumberOfDiskWithZip64EOCD = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(4, blockContents.Length - 4));
		zip64EndOfCentralDirectoryLocator.OffsetOfZip64EOCD = BinaryPrimitives.ReadUInt64LittleEndian(blockContents.Slice(8, blockContents.Length - 8));
		zip64EndOfCentralDirectoryLocator.TotalNumberOfDisks = BinaryPrimitives.ReadUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16));
		zip64EOCDLocator = zip64EndOfCentralDirectoryLocator;
		return true;
	}

	public static Zip64EndOfCentralDirectoryLocator TryReadBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[20];
		int bytesRead = stream.ReadAtLeast(span, span.Length, throwOnEndOfStream: false);
		TryReadBlockCore(span, bytesRead, out var zip64EOCDLocator);
		return zip64EOCDLocator;
	}

	private static void WriteBlockCore(Span<byte> blockContents, long zip64EOCDRecordStart)
	{
		SignatureConstantBytes.CopyTo(blockContents.Slice(0, blockContents.Length));
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(4, blockContents.Length - 4), 0u);
		BinaryPrimitives.WriteInt64LittleEndian(blockContents.Slice(8, blockContents.Length - 8), zip64EOCDRecordStart);
		BinaryPrimitives.WriteUInt32LittleEndian(blockContents.Slice(16, blockContents.Length - 16), 1u);
	}

	public static void WriteBlock(Stream stream, long zip64EOCDRecordStart)
	{
		Span<byte> span = stackalloc byte[20];
		WriteBlockCore(span, zip64EOCDRecordStart);
		stream.Write(span);
	}

	public static async Task<Zip64EndOfCentralDirectoryLocator> TryReadBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] blockContents = new byte[20];
		int bytesRead = await stream.ReadAtLeastAsync(blockContents, blockContents.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		TryReadBlockCore(blockContents, bytesRead, out var zip64EOCDLocator);
		return zip64EOCDLocator;
	}

	public static ValueTask WriteBlockAsync(Stream stream, long zip64EOCDRecordStart, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] array = new byte[20];
		WriteBlockCore(array, zip64EOCDRecordStart);
		return stream.WriteAsync(array, cancellationToken);
	}
}
