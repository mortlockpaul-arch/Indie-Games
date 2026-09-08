using Internal.Cryptography;

namespace System.Security.Cryptography;

public static class HKDF
{
	public static byte[] Extract(HashAlgorithmName hashAlgorithmName, byte[] ikm, byte[]? salt = null)
	{
		ArgumentNullException.ThrowIfNull(ikm, "ikm");
		int num = Helpers.HashLength(hashAlgorithmName);
		byte[] array = new byte[num];
		Extract(hashAlgorithmName, num, ikm, salt, array);
		return array;
	}

	public static int Extract(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
	{
		int num = Helpers.HashLength(hashAlgorithmName);
		if (prk.Length < num)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_Prk_TooSmall, num), "prk");
		}
		if (prk.Length > num)
		{
			prk = prk.Slice(0, num);
		}
		Extract(hashAlgorithmName, num, ikm, salt, prk);
		return num;
	}

	public static byte[] Expand(HashAlgorithmName hashAlgorithmName, byte[] prk, int outputLength, byte[]? info = null)
	{
		ArgumentNullException.ThrowIfNull(prk, "prk");
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(outputLength, "outputLength");
		int num = Helpers.HashLength(hashAlgorithmName);
		if (prk.Length < num)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_Prk_TooSmall, num), "prk");
		}
		int num2 = 255 * num;
		if (outputLength <= 0 || outputLength > num2)
		{
			throw new ArgumentOutOfRangeException("outputLength", System.SR.Format(System.SR.Cryptography_Okm_TooLarge, num2));
		}
		byte[] array = new byte[outputLength];
		Expand(hashAlgorithmName, num, prk, array, info);
		return array;
	}

	public static void Expand(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
	{
		int num = Helpers.HashLength(hashAlgorithmName);
		if (output.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "output");
		}
		if (prk.Length < num)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_Prk_TooSmall, num), "prk");
		}
		int num2 = 255 * num;
		if (output.Length > num2)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_Okm_TooLarge, num2), "output");
		}
		Expand(hashAlgorithmName, num, prk, output, info);
	}

	public static byte[] DeriveKey(HashAlgorithmName hashAlgorithmName, byte[] ikm, int outputLength, byte[]? salt = null, byte[]? info = null)
	{
		ArgumentNullException.ThrowIfNull(ikm, "ikm");
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(outputLength, "outputLength");
		int num = Helpers.HashLength(hashAlgorithmName);
		int num2 = 255 * num;
		if (outputLength > num2)
		{
			throw new ArgumentOutOfRangeException("outputLength", System.SR.Format(System.SR.Cryptography_Okm_TooLarge, num2));
		}
		byte[] array = new byte[outputLength];
		DeriveKeyCore(hashAlgorithmName, num, ikm, array, salt, info);
		return array;
	}

	public static void DeriveKey(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, Span<byte> output, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info)
	{
		int num = Helpers.HashLength(hashAlgorithmName);
		if (output.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "output");
		}
		int num2 = 255 * num;
		if (output.Length > num2)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_Okm_TooLarge, num2), "output");
		}
		DeriveKeyCore(hashAlgorithmName, num, ikm, output, salt, info);
	}

	private static void Extract(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
	{
		HKDFManagedImplementation.Extract(hashAlgorithmName, hashLength, ikm, salt, prk);
	}

	private static void Expand(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
	{
		HKDFManagedImplementation.Expand(hashAlgorithmName, hashLength, prk, output, info);
	}

	private static void DeriveKeyCore(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> ikm, Span<byte> output, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info)
	{
		HKDFManagedImplementation.DeriveKey(hashAlgorithmName, hashLength, ikm, output, salt, info);
	}
}
