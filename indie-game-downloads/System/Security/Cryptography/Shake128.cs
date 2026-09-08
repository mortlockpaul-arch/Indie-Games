using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography;

public sealed class Shake128 : IDisposable
{
	private readonly LiteXof _hashProvider;

	private bool _disposed;

	private ConcurrencyBlock _block;

	private bool _reading;

	public static bool IsSupported { get; } = HashProviderDispenser.HashSupported("CSHAKE128");

	public Shake128()
	{
		CheckPlatformSupport();
		_hashProvider = LiteHashProvider.CreateXof("CSHAKE128");
	}

	internal Shake128(LiteXof hashProvider)
	{
		_hashProvider = hashProvider;
	}

	public void AppendData(byte[] data)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		AppendData(new ReadOnlySpan<byte>(data));
	}

	public void AppendData(ReadOnlySpan<byte> data)
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			_hashProvider.Append(data);
		}
	}

	public byte[] GetHashAndReset(int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			byte[] array = new byte[outputLength];
			_hashProvider.FinalizeAndReset(array);
			return array;
		}
	}

	public void GetHashAndReset(Span<byte> destination)
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			_hashProvider.FinalizeAndReset(destination);
		}
	}

	public byte[] GetCurrentHash(int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			byte[] array = new byte[outputLength];
			_hashProvider.Current(array);
			return array;
		}
	}

	public void GetCurrentHash(Span<byte> destination)
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			_hashProvider.Current(destination);
		}
	}

	public byte[] Read(int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			byte[] array = new byte[outputLength];
			_hashProvider.Read(array);
			_reading = true;
			return array;
		}
	}

	public void Read(Span<byte> destination)
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			_hashProvider.Read(destination);
			_reading = true;
		}
	}

	public void Reset()
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			_hashProvider.Reset();
			_reading = false;
		}
	}

	public Shake128 Clone()
	{
		CheckDisposed();
		using (ConcurrencyBlock.Enter(ref _block))
		{
			CheckReading();
			return new Shake128(_hashProvider.Clone());
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_hashProvider.Dispose();
			_disposed = true;
		}
	}

	public static byte[] HashData(byte[] source, int outputLength)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return HashData(new ReadOnlySpan<byte>(source), outputLength);
	}

	public static byte[] HashData(ReadOnlySpan<byte> source, int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckPlatformSupport();
		byte[] array = new byte[outputLength];
		HashDataCore(source, array);
		return array;
	}

	public static void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		CheckPlatformSupport();
		HashDataCore(source, destination);
	}

	public static byte[] HashData(Stream source, int outputLength)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
		CheckPlatformSupport();
		return LiteHashProvider.XofStream("CSHAKE128", outputLength, source);
	}

	public static void HashData(Stream source, Span<byte> destination)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
		CheckPlatformSupport();
		LiteHashProvider.XofStream("CSHAKE128", source, destination);
	}

	public static ValueTask HashDataAsync(Stream source, Memory<byte> destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
		CheckPlatformSupport();
		return LiteHashProvider.XofStreamAsync("CSHAKE128", source, destination, cancellationToken);
	}

	public static ValueTask<byte[]> HashDataAsync(Stream source, int outputLength, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
		CheckPlatformSupport();
		return LiteHashProvider.XofStreamAsync("CSHAKE128", outputLength, source, cancellationToken);
	}

	private static void HashDataCore(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		HashProviderDispenser.OneShotHashProvider.HashDataXof("CSHAKE128", source, destination);
	}

	private static void CheckPlatformSupport()
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException();
		}
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}

	private void CheckReading()
	{
		if (_reading)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_AlreadyReading);
		}
	}
}
