using System.Buffers;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression;

public struct BrotliEncoder : IDisposable
{
	internal SafeBrotliEncoderHandle _state = global::Interop.Brotli.BrotliEncoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

	private bool _disposed = false;

	public BrotliEncoder(int quality, int window)
	{
		if (_state.IsInvalid)
		{
			throw new IOException(System.SR.BrotliEncoder_Create);
		}
		SetQuality(quality);
		SetWindow(window);
	}

	internal void InitializeEncoder()
	{
		EnsureNotDisposed();
		_state = global::Interop.Brotli.BrotliEncoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		if (_state.IsInvalid)
		{
			throw new IOException(System.SR.BrotliEncoder_Create);
		}
	}

	internal void EnsureInitialized()
	{
		EnsureNotDisposed();
		if (_state == null)
		{
			InitializeEncoder();
		}
	}

	public void Dispose()
	{
		_disposed = true;
		_state?.Dispose();
	}

	private void EnsureNotDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("BrotliEncoder", System.SR.BrotliEncoder_Disposed);
		}
	}

	internal void SetQuality(int quality)
	{
		EnsureNotDisposed();
		if (_state == null || _state.IsInvalid || _state.IsClosed)
		{
			InitializeEncoder();
		}
		if (quality < 0 || quality > 11)
		{
			throw new ArgumentOutOfRangeException("quality", System.SR.Format(System.SR.BrotliEncoder_Quality, quality, 0, 11));
		}
		if (global::Interop.Brotli.BrotliEncoderSetParameter(_state, BrotliEncoderParameter.Quality, (uint)quality) == global::Interop.BOOL.FALSE)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.BrotliEncoder_InvalidSetParameter, "Quality"));
		}
	}

	internal void SetWindow(int window)
	{
		EnsureNotDisposed();
		if (_state == null || _state.IsInvalid || _state.IsClosed)
		{
			InitializeEncoder();
		}
		if (window < 10 || window > 24)
		{
			throw new ArgumentOutOfRangeException("window", System.SR.Format(System.SR.BrotliEncoder_Window, window, 10, 24));
		}
		if (global::Interop.Brotli.BrotliEncoderSetParameter(_state, BrotliEncoderParameter.LGWin, (uint)window) == global::Interop.BOOL.FALSE)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.BrotliEncoder_InvalidSetParameter, "Window"));
		}
	}

	public static int GetMaxCompressedLength(int inputSize)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(inputSize, "inputSize");
		nuint num = global::Interop.Brotli.BrotliEncoderMaxCompressedSize((nuint)inputSize);
		if (num > int.MaxValue)
		{
			throw new ArgumentOutOfRangeException("inputSize");
		}
		return (int)num;
	}

	internal OperationStatus Flush(Memory<byte> destination, out int bytesWritten)
	{
		return Flush(destination.Span, out bytesWritten);
	}

	public OperationStatus Flush(Span<byte> destination, out int bytesWritten)
	{
		int bytesConsumed;
		return Compress(ReadOnlySpan<byte>.Empty, destination, out bytesConsumed, out bytesWritten, BrotliEncoderOperation.Flush);
	}

	internal OperationStatus Compress(ReadOnlyMemory<byte> source, Memory<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
	{
		return Compress(source.Span, destination.Span, out bytesConsumed, out bytesWritten, isFinalBlock);
	}

	public OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
	{
		return Compress(source, destination, out bytesConsumed, out bytesWritten, isFinalBlock ? BrotliEncoderOperation.Finish : BrotliEncoderOperation.Process);
	}

	internal unsafe OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, BrotliEncoderOperation operation)
	{
		EnsureInitialized();
		bytesWritten = 0;
		bytesConsumed = 0;
		nuint availableOut = (nuint)destination.Length;
		nuint availableIn = (nuint)source.Length;
		while ((int)availableOut > 0)
		{
			fixed (byte* reference = &MemoryMarshal.GetReference(source))
			{
				byte* ptr = reference;
				fixed (byte* reference2 = &MemoryMarshal.GetReference(destination))
				{
					byte* ptr2 = reference2;
					if (global::Interop.Brotli.BrotliEncoderCompressStream(_state, operation, ref availableIn, &ptr, ref availableOut, &ptr2, out var _) == global::Interop.BOOL.FALSE)
					{
						return OperationStatus.InvalidData;
					}
					bytesConsumed += source.Length - (int)availableIn;
					bytesWritten += destination.Length - (int)availableOut;
					if ((int)availableOut == destination.Length && global::Interop.Brotli.BrotliEncoderHasMoreOutput(_state) == global::Interop.BOOL.FALSE && availableIn == 0)
					{
						return OperationStatus.Done;
					}
					source = source.Slice(source.Length - (int)availableIn);
					destination = destination.Slice(destination.Length - (int)availableOut);
				}
			}
		}
		return OperationStatus.DestinationTooSmall;
	}

	public static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		return TryCompress(source, destination, out bytesWritten, 4, 22);
	}

	public unsafe static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, int quality, int window)
	{
		if (quality < 0 || quality > 11)
		{
			throw new ArgumentOutOfRangeException("quality", System.SR.Format(System.SR.BrotliEncoder_Quality, quality, 0, 11));
		}
		if (window < 10 || window > 24)
		{
			throw new ArgumentOutOfRangeException("window", System.SR.Format(System.SR.BrotliEncoder_Window, window, 10, 24));
		}
		fixed (byte* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (byte* reference2 = &MemoryMarshal.GetReference(destination))
			{
				nuint num = (nuint)destination.Length;
				bool result = global::Interop.Brotli.BrotliEncoderCompress(quality, window, 0, (nuint)source.Length, reference, &num, reference2) != global::Interop.BOOL.FALSE;
				bytesWritten = (int)num;
				return result;
			}
		}
	}
}
