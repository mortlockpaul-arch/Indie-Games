using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography;

internal static class MLKemPkcs8
{
	internal static bool TryExportPkcs8PrivateKey(MLKem kem, bool hasSeed, bool hasDecapsulationKey, Span<byte> destination, out int bytesWritten)
	{
		AlgorithmIdentifierAsn algorithmIdentifierAsn = new AlgorithmIdentifierAsn
		{
			Algorithm = kem.Algorithm.Oid,
			Parameters = null
		};
		MLKemPrivateKeyAsn mLKemPrivateKeyAsn = default(MLKemPrivateKeyAsn);
		byte[] array = null;
		int clearSize = 0;
		try
		{
			if (hasSeed)
			{
				int privateSeedSizeInBytes = kem.Algorithm.PrivateSeedSizeInBytes;
				array = System.Security.Cryptography.CryptoPool.Rent(privateSeedSizeInBytes);
				Memory<byte> memory = array.AsMemory(0, privateSeedSizeInBytes);
				kem.ExportPrivateSeed(memory.Span);
				clearSize = memory.Length;
				mLKemPrivateKeyAsn.Seed = memory;
			}
			else
			{
				if (!hasDecapsulationKey)
				{
					throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
				}
				int decapsulationKeySizeInBytes = kem.Algorithm.DecapsulationKeySizeInBytes;
				array = System.Security.Cryptography.CryptoPool.Rent(decapsulationKeySizeInBytes);
				Memory<byte> memory2 = array.AsMemory(0, decapsulationKeySizeInBytes);
				kem.ExportDecapsulationKey(memory2.Span);
				clearSize = memory2.Length;
				mLKemPrivateKeyAsn.ExpandedKey = memory2;
			}
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			algorithmIdentifierAsn.Encode(asnWriter);
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER);
			mLKemPrivateKeyAsn.Encode(asnWriter2);
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
