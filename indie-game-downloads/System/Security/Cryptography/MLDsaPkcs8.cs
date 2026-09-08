using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography;

internal static class MLDsaPkcs8
{
	internal static bool TryExportPkcs8PrivateKey(MLDsa dsa, bool hasSeed, bool hasPrivateKey, Span<byte> destination, out int bytesWritten)
	{
		AlgorithmIdentifierAsn algorithmIdentifierAsn = new AlgorithmIdentifierAsn
		{
			Algorithm = dsa.Algorithm.Oid,
			Parameters = null
		};
		MLDsaPrivateKeyAsn mLDsaPrivateKeyAsn = default(MLDsaPrivateKeyAsn);
		byte[] array = null;
		int clearSize = 0;
		try
		{
			if (hasSeed)
			{
				int privateSeedSizeInBytes = dsa.Algorithm.PrivateSeedSizeInBytes;
				array = System.Security.Cryptography.CryptoPool.Rent(privateSeedSizeInBytes);
				Memory<byte> memory = array.AsMemory(0, privateSeedSizeInBytes);
				dsa.ExportMLDsaPrivateSeed(memory.Span);
				clearSize = memory.Length;
				mLDsaPrivateKeyAsn.Seed = memory;
			}
			else
			{
				if (!hasPrivateKey)
				{
					throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
				}
				int privateKeySizeInBytes = dsa.Algorithm.PrivateKeySizeInBytes;
				array = System.Security.Cryptography.CryptoPool.Rent(privateKeySizeInBytes);
				Memory<byte> memory2 = array.AsMemory(0, privateKeySizeInBytes);
				dsa.ExportMLDsaPrivateKey(memory2.Span);
				clearSize = memory2.Length;
				mLDsaPrivateKeyAsn.ExpandedKey = memory2;
			}
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			algorithmIdentifierAsn.Encode(asnWriter);
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER);
			mLDsaPrivateKeyAsn.Encode(asnWriter2);
			AsnWriter asnWriter3 = KeyFormatHelper.WritePkcs8(asnWriter, asnWriter2);
			bool result = asnWriter3.TryEncode(destination, out bytesWritten);
			asnWriter2.Reset();
			asnWriter3.Reset();
			return result;
		}
		finally
		{
			if (array != null)
			{
				System.Security.Cryptography.CryptoPool.Return(array, clearSize);
			}
		}
	}
}
