using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography;

internal static class LiteHashProvider
{
	internal static int HashStream(string hashAlgorithmId, Stream source, Span<byte> destination)
	{
		return ProcessStream(CreateHash(hashAlgorithmId), source, destination);
	}

	internal static byte[] HashStream(string hashAlgorithmId, int hashSizeInBytes, Stream source)
	{
		byte[] array = new byte[hashSizeInBytes];
		ProcessStream(CreateHash(hashAlgorithmId), source, array);
		return array;
	}

	internal static ValueTask<int> HashStreamAsync(string hashAlgorithmId, Stream source, Memory<byte> destination, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<int>(cancellationToken);
		}
		return ProcessStreamAsync(CreateHash(hashAlgorithmId), source, destination, cancellationToken);
	}

	internal static ValueTask<byte[]> HashStreamAsync(string hashAlgorithmId, Stream source, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<byte[]>(cancellationToken);
		}
		LiteHash hash = CreateHash(hashAlgorithmId);
		return ProcessStreamAsync(hash, hash.HashSizeInBytes, source, cancellationToken);
	}

	internal static int HmacStream(string hashAlgorithmId, ReadOnlySpan<byte> key, Stream source, Span<byte> destination)
	{
		return ProcessStream(CreateHmac(hashAlgorithmId, key), source, destination);
	}

	internal static byte[] HmacStream(string hashAlgorithmId, int hashSizeInBytes, ReadOnlySpan<byte> key, Stream source)
	{
		byte[] array = new byte[hashSizeInBytes];
		ProcessStream(CreateHmac(hashAlgorithmId, key), source, array);
		return array;
	}

	internal static ValueTask<int> HmacStreamAsync(string hashAlgorithmId, ReadOnlySpan<byte> key, Stream source, Memory<byte> destination, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<int>(cancellationToken);
		}
		return ProcessStreamAsync(CreateHmac(hashAlgorithmId, key), source, destination, cancellationToken);
	}

	internal static ValueTask<byte[]> HmacStreamAsync(string hashAlgorithmId, ReadOnlySpan<byte> key, Stream source, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<byte[]>(cancellationToken);
		}
		LiteHmac hash = CreateHmac(hashAlgorithmId, key);
		return ProcessStreamAsync(hash, hash.HashSizeInBytes, source, cancellationToken);
	}

	private static int ProcessStream<T>(T hash, Stream source, Span<byte> destination) where T : ILiteHash
	{
		using (hash)
		{
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(4096);
			int num = 0;
			try
			{
				int num2;
				while ((num2 = source.Read(array)) > 0)
				{
					num = Math.Max(num, num2);
					hash.Append(array.AsSpan(0, num2));
				}
				return hash.Finalize(destination);
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(array, num);
			}
		}
	}

	private static async ValueTask<int> ProcessStreamAsync<T>(T hash, Stream source, Memory<byte> destination, CancellationToken cancellationToken) where T : ILiteHash
	{
		using (hash)
		{
			byte[] rented = System.Security.Cryptography.CryptoPool.Rent(4096);
			int maxRead = 0;
			try
			{
				int num;
				T val;
				while ((num = await source.ReadAsync(rented, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) > 0)
				{
					maxRead = Math.Max(maxRead, num);
					ref T reference = ref hash;
					val = default(T);
					if (val == null)
					{
						val = reference;
						reference = ref val;
					}
					ReadOnlySpan<byte> data = rented.AsSpan(0, num);
					reference.Append(data);
				}
				ref T reference2 = ref hash;
				val = default(T);
				if (val == null)
				{
					val = reference2;
					reference2 = ref val;
				}
				Span<byte> span = destination.Span;
				return reference2.Finalize(span);
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(rented, maxRead);
			}
		}
	}

	private static async ValueTask<byte[]> ProcessStreamAsync<T>(T hash, int outputLength, Stream source, CancellationToken cancellationToken) where T : ILiteHash
	{
		byte[] result = new byte[outputLength];
		await ProcessStreamAsync(hash, source, result, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return result;
	}

	internal static void XofStream(string hashAlgorithmId, Stream source, Span<byte> destination)
	{
		ProcessStream(CreateXof(hashAlgorithmId), source, destination);
	}

	internal static byte[] XofStream(string hashAlgorithmId, int outputLength, Stream source)
	{
		byte[] array = new byte[outputLength];
		ProcessStream(CreateXof(hashAlgorithmId), source, array);
		return array;
	}

	internal static void KmacStream(string hashAlgorithmId, ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString, Stream source, bool xof, Span<byte> destination)
	{
		ProcessStream(CreateKmac(hashAlgorithmId, key, customizationString, xof), source, destination);
	}

	internal static byte[] KmacStream(string hashAlgorithmId, ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString, int outputLength, Stream source, bool xof)
	{
		byte[] array = new byte[outputLength];
		ProcessStream(CreateKmac(hashAlgorithmId, key, customizationString, xof), source, array);
		return array;
	}

	internal static ValueTask XofStreamAsync(string hashAlgorithmId, Stream source, Memory<byte> destination, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled(cancellationToken);
		}
		return ProcessStreamIndefiniteAsync(CreateXof(hashAlgorithmId), source, destination, cancellationToken);
	}

	internal static ValueTask<byte[]> XofStreamAsync(string hashAlgorithmId, int outputLength, Stream source, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<byte[]>(cancellationToken);
		}
		return ProcessStreamAsync(CreateXof(hashAlgorithmId), outputLength, source, cancellationToken);
	}

	internal static ValueTask KmacStreamAsync(string hashAlgorithmId, ReadOnlySpan<byte> key, Stream source, bool xof, Memory<byte> destination, ReadOnlySpan<byte> customizationString, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled(cancellationToken);
		}
		return ProcessStreamIndefiniteAsync(CreateKmac(hashAlgorithmId, key, customizationString, xof), source, destination, cancellationToken);
	}

	internal static ValueTask<byte[]> KmacStreamAsync(string hashAlgorithmId, ReadOnlySpan<byte> key, Stream source, bool xof, int outputLength, ReadOnlySpan<byte> customizationString, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return ValueTask.FromCanceled<byte[]>(cancellationToken);
		}
		return ProcessStreamAsync(CreateKmac(hashAlgorithmId, key, customizationString, xof), outputLength, source, cancellationToken);
	}

	private static async ValueTask ProcessStreamIndefiniteAsync<T>(T hash, Stream source, Memory<byte> destination, CancellationToken cancellationToken) where T : ILiteHash
	{
		using (hash)
		{
			byte[] rented = System.Security.Cryptography.CryptoPool.Rent(4096);
			int maxRead = 0;
			try
			{
				int num;
				T val;
				while ((num = await source.ReadAsync(rented, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) > 0)
				{
					maxRead = Math.Max(maxRead, num);
					ref T reference = ref hash;
					val = default(T);
					if (val == null)
					{
						val = reference;
						reference = ref val;
					}
					ReadOnlySpan<byte> data = rented.AsSpan(0, num);
					reference.Append(data);
				}
				ref T reference2 = ref hash;
				val = default(T);
				if (val == null)
				{
					val = reference2;
					reference2 = ref val;
				}
				Span<byte> span = destination.Span;
				reference2.Finalize(span);
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(rented, maxRead);
			}
		}
	}

	private static LiteHash CreateHash(string hashAlgorithmId)
	{
		return new LiteHash(hashAlgorithmId);
	}

	private static LiteHmac CreateHmac(string hashAlgorithmId, ReadOnlySpan<byte> key)
	{
		return new LiteHmac(hashAlgorithmId, key);
	}

	internal static LiteXof CreateXof(string hashAlgorithmId)
	{
		return new LiteXof(hashAlgorithmId);
	}

	internal static LiteKmac CreateKmac(string algorithmId, ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString, bool xof)
	{
		return new LiteKmac(algorithmId, key, customizationString, xof);
	}
}
