using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct ZipLocalFileHeader
{
	public static ReadOnlySpan<byte> DataDescriptorSignatureConstantBytes => new byte[4] { 80, 75, 7, 8 };

	public static ReadOnlySpan<byte> SignatureConstantBytes => new byte[4] { 80, 75, 3, 4 };

	private static void GetExtraFieldsInitialize(Stream stream, out int relativeFilenameLengthLocation, out int relativeExtraFieldLengthLocation)
	{
		relativeFilenameLengthLocation = 0;
		relativeExtraFieldLengthLocation = 2;
		stream.Seek(26L, SeekOrigin.Current);
	}

	private static void GetExtraFieldsCore(Span<byte> fixedHeaderBuffer, int relativeFilenameLengthLocation, int relativeExtraFieldLengthLocation, out ushort filenameLength, out ushort extraFieldLength)
	{
		int num = relativeFilenameLengthLocation;
		filenameLength = BinaryPrimitives.ReadUInt16LittleEndian(fixedHeaderBuffer.Slice(num, fixedHeaderBuffer.Length - num));
		num = relativeExtraFieldLengthLocation;
		extraFieldLength = BinaryPrimitives.ReadUInt16LittleEndian(fixedHeaderBuffer.Slice(num, fixedHeaderBuffer.Length - num));
	}

	private static List<ZipGenericExtraField> GetExtraFieldPostReadWork(Span<byte> extraFieldBuffer, out byte[] trailingData)
	{
		List<ZipGenericExtraField> list = ZipGenericExtraField.ParseExtraField(extraFieldBuffer, out var trailingExtraFieldData);
		Zip64ExtraField.RemoveZip64Blocks(list);
		trailingData = trailingExtraFieldData.ToArray();
		return list;
	}

	public static List<ZipGenericExtraField> GetExtraFields(Stream stream, out byte[] trailingData)
	{
		Span<byte> span = stackalloc byte[4];
		GetExtraFieldsInitialize(stream, out var relativeFilenameLengthLocation, out var relativeExtraFieldLengthLocation);
		stream.ReadExactly(span);
		GetExtraFieldsCore(span, relativeFilenameLengthLocation, relativeExtraFieldLengthLocation, out var filenameLength, out var extraFieldLength);
		byte[] array = ((extraFieldLength > 512) ? ArrayPool<byte>.Shared.Rent(extraFieldLength) : null);
		Span<byte> span2 = ((extraFieldLength > 512) ? array.AsSpan(0, extraFieldLength) : stackalloc byte[512].Slice(0, extraFieldLength));
		Span<byte> span3 = span2;
		try
		{
			stream.Seek(filenameLength, SeekOrigin.Current);
			stream.ReadExactly(span3);
			return GetExtraFieldPostReadWork(span3, out trailingData);
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	private static bool TrySkipBlockCore(Stream stream, Span<byte> blockBytes, int bytesRead, long currPosition)
	{
		if (bytesRead != 4 || !((ReadOnlySpan<byte>)blockBytes).SequenceEqual(SignatureConstantBytes))
		{
			return false;
		}
		if (stream.Length < currPosition + 26)
		{
			return false;
		}
		stream.Seek(22L, SeekOrigin.Current);
		return true;
	}

	private static bool TrySkipBlockFinalize(Stream stream, Span<byte> blockBytes, int bytesRead)
	{
		if (bytesRead != 4)
		{
			return false;
		}
		int num = 2;
		int num2 = 0;
		ushort num3 = BinaryPrimitives.ReadUInt16LittleEndian(blockBytes.Slice(num2, blockBytes.Length - num2));
		num2 = num;
		ushort num4 = BinaryPrimitives.ReadUInt16LittleEndian(blockBytes.Slice(num2, blockBytes.Length - num2));
		if (stream.Length < stream.Position + num3 + num4)
		{
			return false;
		}
		stream.Seek(num3 + num4, SeekOrigin.Current);
		return true;
	}

	public static bool TrySkipBlock(Stream stream)
	{
		Span<byte> span = stackalloc byte[4];
		long position = stream.Position;
		int bytesRead = stream.ReadAtLeast(span, span.Length, throwOnEndOfStream: false);
		if (!TrySkipBlockCore(stream, span, bytesRead, position))
		{
			return false;
		}
		bytesRead = stream.ReadAtLeast(span, span.Length, throwOnEndOfStream: false);
		return TrySkipBlockFinalize(stream, span, bytesRead);
	}

	public static async Task<(List<ZipGenericExtraField>, byte[] trailingData)> GetExtraFieldsAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] fixedHeaderBuffer = new byte[4];
		GetExtraFieldsInitialize(stream, out var relativeFilenameLengthLocation, out var relativeExtraFieldLengthLocation);
		await stream.ReadExactlyAsync(fixedHeaderBuffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		GetExtraFieldsCore(fixedHeaderBuffer, relativeFilenameLengthLocation, relativeExtraFieldLengthLocation, out var filenameLength, out var extraFieldLength);
		byte[] arrayPoolBuffer = ArrayPool<byte>.Shared.Rent(extraFieldLength);
		Memory<byte> extraFieldBuffer = arrayPoolBuffer.AsMemory(0, extraFieldLength);
		try
		{
			stream.Seek(filenameLength, SeekOrigin.Current);
			await stream.ReadExactlyAsync(extraFieldBuffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			byte[] trailingData;
			return (GetExtraFieldPostReadWork(extraFieldBuffer.Span, out trailingData), trailingData: trailingData);
		}
		finally
		{
			if (arrayPoolBuffer != null)
			{
				ArrayPool<byte>.Shared.Return(arrayPoolBuffer);
			}
		}
	}

	public static async Task<bool> TrySkipBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		byte[] blockBytes = new byte[4];
		long currPosition = stream.Position;
		int bytesRead = await stream.ReadAtLeastAsync(blockBytes, blockBytes.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!TrySkipBlockCore(stream, blockBytes, bytesRead, currPosition))
		{
			return false;
		}
		bytesRead = await stream.ReadAtLeastAsync(blockBytes, blockBytes.Length, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return TrySkipBlockFinalize(stream, blockBytes, bytesRead);
	}
}
