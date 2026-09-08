using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Internal.Cryptography;

namespace System.Security.Cryptography;

public static class CryptographicOperations
{
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		if (left.Length != right.Length)
		{
			return false;
		}
		int length = left.Length;
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			num |= left[i] - right[i];
		}
		return num == 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static void ZeroMemory(Span<byte> buffer)
	{
		buffer.Clear();
	}

	public static byte[] HashData(HashAlgorithmName hashAlgorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return HashData(hashAlgorithm, new ReadOnlySpan<byte>(source));
	}

	public static byte[] HashData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> source)
	{
		byte[] array = new byte[CheckHashAndGetLength(hashAlgorithm)];
		HashProviderDispenser.OneShotHashProvider.HashData(hashAlgorithm.Name, source, array);
		return array;
	}

	public static int HashData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> source, Span<byte> destination)
	{
		if (!TryHashData(hashAlgorithm, source, destination, out var bytesWritten))
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "destination");
		}
		return bytesWritten;
	}

	public static bool TryHashData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		int num = CheckHashAndGetLength(hashAlgorithm);
		if (destination.Length < num)
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten = HashProviderDispenser.OneShotHashProvider.HashData(hashAlgorithm.Name, source, destination);
		return true;
	}

	public static byte[] HashData(HashAlgorithmName hashAlgorithm, Stream source)
	{
		int hashSizeInBytes = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		return LiteHashProvider.HashStream(hashAlgorithm.Name, hashSizeInBytes, source);
	}

	public static int HashData(HashAlgorithmName hashAlgorithm, Stream source, Span<byte> destination)
	{
		int requiredSize = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		CheckDestinationSize(requiredSize, destination.Length);
		return LiteHashProvider.HashStream(hashAlgorithm.Name, source, destination);
	}

	public static ValueTask<int> HashDataAsync(HashAlgorithmName hashAlgorithm, Stream source, Memory<byte> destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		int requiredSize = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		CheckDestinationSize(requiredSize, destination.Length);
		return LiteHashProvider.HashStreamAsync(hashAlgorithm.Name, source, destination, cancellationToken);
	}

	public static ValueTask<byte[]> HashDataAsync(HashAlgorithmName hashAlgorithm, Stream source, CancellationToken cancellationToken = default(CancellationToken))
	{
		CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		return LiteHashProvider.HashStreamAsync(hashAlgorithm.Name, source, cancellationToken);
	}

	public static byte[] HmacData(HashAlgorithmName hashAlgorithm, byte[] key, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentNullException.ThrowIfNull(source, "source");
		return HmacData(hashAlgorithm, new ReadOnlySpan<byte>(key), new ReadOnlySpan<byte>(source));
	}

	public static byte[] HmacData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> key, ReadOnlySpan<byte> source)
	{
		byte[] array = new byte[CheckHashAndGetLength(hashAlgorithm)];
		HashProviderDispenser.OneShotHashProvider.MacData(hashAlgorithm.Name, key, source, array);
		return array;
	}

	public static int HmacData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination)
	{
		if (!TryHmacData(hashAlgorithm, key, source, destination, out var bytesWritten))
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "destination");
		}
		return bytesWritten;
	}

	public static bool TryHmacData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> key, ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		int num = CheckHashAndGetLength(hashAlgorithm);
		if (destination.Length < num)
		{
			bytesWritten = 0;
			return false;
		}
		bytesWritten = HashProviderDispenser.OneShotHashProvider.MacData(hashAlgorithm.Name, key, source, destination);
		return true;
	}

	public static byte[] HmacData(HashAlgorithmName hashAlgorithm, byte[] key, Stream source)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return HmacData(hashAlgorithm, new ReadOnlySpan<byte>(key), source);
	}

	public static byte[] HmacData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> key, Stream source)
	{
		int hashSizeInBytes = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		return LiteHashProvider.HmacStream(hashAlgorithm.Name, hashSizeInBytes, key, source);
	}

	public static int HmacData(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> key, Stream source, Span<byte> destination)
	{
		int requiredSize = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		CheckDestinationSize(requiredSize, destination.Length);
		return LiteHashProvider.HmacStream(hashAlgorithm.Name, key, source, destination);
	}

	public static ValueTask<byte[]> HmacDataAsync(HashAlgorithmName hashAlgorithm, byte[] key, Stream source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return HmacDataAsync(hashAlgorithm, new ReadOnlyMemory<byte>(key), source, cancellationToken);
	}

	public static ValueTask<int> HmacDataAsync(HashAlgorithmName hashAlgorithm, ReadOnlyMemory<byte> key, Stream source, Memory<byte> destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		int requiredSize = CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		CheckDestinationSize(requiredSize, destination.Length);
		return LiteHashProvider.HmacStreamAsync(hashAlgorithm.Name, key.Span, source, destination, cancellationToken);
	}

	public static ValueTask<byte[]> HmacDataAsync(HashAlgorithmName hashAlgorithm, ReadOnlyMemory<byte> key, Stream source, CancellationToken cancellationToken = default(CancellationToken))
	{
		CheckHashAndGetLength(hashAlgorithm);
		CheckStream(source);
		return LiteHashProvider.HmacStreamAsync(hashAlgorithm.Name, key.Span, source, cancellationToken);
	}

	private static void CheckStream([NotNull] Stream source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.CanRead)
		{
			throw new ArgumentException(System.SR.Argument_StreamNotReadable, "source");
		}
	}

	private static int CheckHashAndGetLength(HashAlgorithmName hashAlgorithm)
	{
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		switch (hashAlgorithm.Name)
		{
		case "SHA256":
			return 32;
		case "SHA1":
			return 20;
		case "SHA512":
			return 64;
		case "SHA384":
			return 48;
		case "SHA3-256":
			if (!HashProviderDispenser.HashSupported("SHA3-256"))
			{
				throw new PlatformNotSupportedException();
			}
			return 32;
		case "SHA3-384":
			if (!HashProviderDispenser.HashSupported("SHA3-384"))
			{
				throw new PlatformNotSupportedException();
			}
			return 48;
		case "SHA3-512":
			if (!HashProviderDispenser.HashSupported("SHA3-512"))
			{
				throw new PlatformNotSupportedException();
			}
			return 64;
		case "MD5":
			if (Helpers.HasMD5)
			{
				return 16;
			}
			break;
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name));
	}

	private static void CheckDestinationSize(int requiredSize, int destinationSize)
	{
		if (destinationSize < requiredSize)
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "destination");
		}
	}
}
