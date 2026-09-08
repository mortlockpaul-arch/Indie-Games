using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Internal.Cryptography;

namespace System.Security.Cryptography;

public sealed class KmacXof256 : IDisposable
{
	private ConcurrentSafeKmac _kmacProvider;

	private bool _disposed;

	public static bool IsSupported { get; } = HashProviderDispenser.KmacSupported("KMAC256");

	public KmacXof256(byte[] key, byte[]? customizationString = null)
		: this(Helpers.ArrayToSpanOrThrow(key, "key"), customizationString)
	{
	}

	public KmacXof256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString = default(ReadOnlySpan<byte>))
	{
		CheckPlatformSupport();
		_kmacProvider = new ConcurrentSafeKmac("KMAC256", key, customizationString, xof: true);
	}

	private KmacXof256(ConcurrentSafeKmac kmacProvider)
	{
		_kmacProvider = kmacProvider;
	}

	public void AppendData(byte[] data)
	{
		AppendData(Helpers.ArrayToSpanOrThrow(data, "data"));
	}

	public void AppendData(ReadOnlySpan<byte> data)
	{
		CheckDisposed();
		_kmacProvider.Append(data);
	}

	public byte[] GetHashAndReset(int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckDisposed();
		byte[] array = new byte[outputLength];
		_kmacProvider.Finalize(array);
		_kmacProvider.Reset();
		return array;
	}

	public void GetHashAndReset(Span<byte> destination)
	{
		CheckDisposed();
		_kmacProvider.Finalize(destination);
		_kmacProvider.Reset();
	}

	public byte[] GetCurrentHash(int outputLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckDisposed();
		byte[] array = new byte[outputLength];
		_kmacProvider.Current(array);
		return array;
	}

	public void GetCurrentHash(Span<byte> destination)
	{
		CheckDisposed();
		_kmacProvider.Current(destination);
	}

	public KmacXof256 Clone()
	{
		CheckDisposed();
		return new KmacXof256(_kmacProvider.Clone());
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_kmacProvider.Dispose();
		}
	}

	public static byte[] HashData(byte[] key, byte[] source, int outputLength, byte[]? customizationString = null)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentNullException.ThrowIfNull(source, "source");
		return HashData(new ReadOnlySpan<byte>(key), new ReadOnlySpan<byte>(source), outputLength, customizationString);
	}

	public static byte[] HashData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, int outputLength, ReadOnlySpan<byte> customizationString = default(ReadOnlySpan<byte>))
	{
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckPlatformSupport();
		byte[] array = new byte[outputLength];
		HashDataCore(key, source, array, customizationString);
		return array;
	}

	public static void HashData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination, ReadOnlySpan<byte> customizationString = default(ReadOnlySpan<byte>))
	{
		CheckPlatformSupport();
		HashDataCore(key, source, destination, customizationString);
	}

	public static byte[] HashData(byte[] key, Stream source, int outputLength, byte[]? customizationString = null)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		return LiteHashProvider.KmacStream("KMAC256", key, customizationString, outputLength, source, xof: true);
	}

	public static byte[] HashData(ReadOnlySpan<byte> key, Stream source, int outputLength, ReadOnlySpan<byte> customizationString = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		return LiteHashProvider.KmacStream("KMAC256", key, customizationString, outputLength, source, xof: true);
	}

	public static void HashData(ReadOnlySpan<byte> key, Stream source, Span<byte> destination, ReadOnlySpan<byte> customizationString = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		LiteHashProvider.KmacStream("KMAC256", key, customizationString, source, xof: true, destination);
	}

	public static ValueTask<byte[]> HashDataAsync(byte[] key, Stream source, int outputLength, byte[]? customizationString = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		return LiteHashProvider.KmacStreamAsync("KMAC256", key, source, xof: true, outputLength, customizationString, cancellationToken);
	}

	public static ValueTask<byte[]> HashDataAsync(ReadOnlyMemory<byte> key, Stream source, int outputLength, ReadOnlyMemory<byte> customizationString = default(ReadOnlyMemory<byte>), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(outputLength, "outputLength");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		return LiteHashProvider.KmacStreamAsync("KMAC256", key.Span, source, xof: true, outputLength, customizationString.Span, cancellationToken);
	}

	public static ValueTask HashDataAsync(ReadOnlyMemory<byte> key, Stream source, Memory<byte> destination, ReadOnlyMemory<byte> customizationString = default(ReadOnlyMemory<byte>), CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		CheckStreamCanRead(source);
		CheckPlatformSupport();
		return LiteHashProvider.KmacStreamAsync("KMAC256", key.Span, source, xof: true, destination, customizationString.Span, cancellationToken);
	}

	private static void HashDataCore(ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination, ReadOnlySpan<byte> customizationString)
	{
		HashProviderDispenser.OneShotHashProvider.KmacData("KMAC256", key, source, destination, customizationString, xof: true);
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}

	private static void CheckPlatformSupport()
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException();
		}
	}

	private static void CheckStreamCanRead(Stream source)
	{
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
	}
}
