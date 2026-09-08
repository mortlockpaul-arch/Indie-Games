using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[RequiresUnreferencedCode("PKCS1MaskGenerationMethod is not trim compatible because the algorithm implementation referenced by HashName might be removed.")]
public class PKCS1MaskGenerationMethod : MaskGenerationMethod
{
	private string _hashNameValue;

	public string HashName
	{
		get
		{
			return _hashNameValue;
		}
		set
		{
			_hashNameValue = value ?? "SHA1";
		}
	}

	public PKCS1MaskGenerationMethod()
	{
		_hashNameValue = "SHA1";
	}

	public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
	{
		using HashAlgorithm hashAlgorithm = CryptoConfig.CreateFromName(_hashNameValue) as HashAlgorithm;
		if (hashAlgorithm == null)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, _hashNameValue));
		}
		byte[] array = new byte[4];
		byte[] array2 = new byte[cbReturn];
		uint num = 0u;
		for (int i = 0; i < array2.Length; i += hashAlgorithm.Hash.Length)
		{
			BinaryPrimitives.WriteUInt32BigEndian(array, num++);
			hashAlgorithm.TransformBlock(rgbSeed, 0, rgbSeed.Length, rgbSeed, 0);
			hashAlgorithm.TransformFinalBlock(array, 0, 4);
			byte[] hash = hashAlgorithm.Hash;
			hashAlgorithm.Initialize();
			Buffer.BlockCopy(hash, 0, array2, i, Math.Min(array2.Length - i, hash.Length));
		}
		return array2;
	}
}
