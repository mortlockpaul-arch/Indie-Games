namespace System.Security.Cryptography;

internal static class AesAEAD
{
	internal static void CheckKeySize(int keySizeInBytes)
	{
		if ((keySizeInBytes != 16 && keySizeInBytes != 24 && keySizeInBytes != 32) || 1 == 0)
		{
			throw new CryptographicException(System.SR.Cryptography_InvalidKeySize);
		}
	}
}
