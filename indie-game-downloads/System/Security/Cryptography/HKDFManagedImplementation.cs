namespace System.Security.Cryptography;

internal static class HKDFManagedImplementation
{
	internal static void Extract(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
	{
		CryptographicOperations.HmacData(hashAlgorithmName, salt, ikm, prk);
	}

	internal static void Expand(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
	{
		byte reference = 0;
		Span<byte> span = new Span<byte>(ref reference);
		Span<byte> span2 = Span<byte>.Empty;
		Span<byte> destination = output;
		Span<byte> span3 = stackalloc byte[64];
		byte[] array = null;
		ReadOnlySpan<byte> data;
		if (((ReadOnlySpan<byte>)output).Overlaps(info))
		{
			if (info.Length > 64)
			{
				array = System.Security.Cryptography.CryptoPool.Rent(info.Length);
				span3 = array;
			}
			span3 = span3.Slice(0, info.Length);
			info.CopyTo(span3);
			data = span3;
		}
		else
		{
			data = info;
		}
		using (IncrementalHash incrementalHash = IncrementalHash.CreateHMAC(hashAlgorithmName, prk))
		{
			int num = 1;
			while (true)
			{
				incrementalHash.AppendData(span2);
				incrementalHash.AppendData(data);
				reference = (byte)num;
				incrementalHash.AppendData(span);
				if (destination.Length < hashLength)
				{
					break;
				}
				span2 = destination.Slice(0, hashLength);
				destination = destination.Slice(hashLength);
				GetHashAndReset(incrementalHash, span2);
				num++;
			}
			if (destination.Length > 0)
			{
				Span<byte> output2 = stackalloc byte[hashLength];
				GetHashAndReset(incrementalHash, output2);
				output2.Slice(0, destination.Length).CopyTo(destination);
			}
		}
		if (array != null)
		{
			System.Security.Cryptography.CryptoPool.Return(array, info.Length);
		}
	}

	internal static void DeriveKey(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> ikm, Span<byte> output, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info)
	{
		Span<byte> span = stackalloc byte[hashLength];
		Extract(hashAlgorithmName, hashLength, ikm, salt, span);
		Expand(hashAlgorithmName, hashLength, span, output, info);
		CryptographicOperations.ZeroMemory(span);
	}

	private static void GetHashAndReset(IncrementalHash hmac, Span<byte> output)
	{
		hmac.GetHashAndReset(output);
	}
}
