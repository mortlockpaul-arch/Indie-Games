using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class RSAPssX509SignatureGenerator : X509SignatureGenerator
{
	private readonly RSA _key;

	private readonly RSASignaturePadding _padding;

	internal RSAPssX509SignatureGenerator(RSA key, RSASignaturePadding padding)
	{
		_key = key;
		_padding = padding;
	}

	public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
	{
		if (_padding != RSASignaturePadding.Pss)
		{
			throw new CryptographicException(System.SR.Cryptography_InvalidPaddingMode);
		}
		int saltLength;
		string text;
		if (hashAlgorithm == HashAlgorithmName.SHA256)
		{
			saltLength = 32;
			text = "2.16.840.1.101.3.4.2.1";
		}
		else if (hashAlgorithm == HashAlgorithmName.SHA384)
		{
			saltLength = 48;
			text = "2.16.840.1.101.3.4.2.2";
		}
		else
		{
			if (!(hashAlgorithm == HashAlgorithmName.SHA512))
			{
				throw new ArgumentOutOfRangeException("hashAlgorithm", hashAlgorithm, System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name));
			}
			saltLength = 64;
			text = "2.16.840.1.101.3.4.2.3";
		}
		PssParamsAsn pssParamsAsn = new PssParamsAsn
		{
			HashAlgorithm = new AlgorithmIdentifierAsn
			{
				Algorithm = text
			},
			MaskGenAlgorithm = new AlgorithmIdentifierAsn
			{
				Algorithm = "1.2.840.113549.1.1.8"
			},
			SaltLength = saltLength,
			TrailerField = 1
		};
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		using (asnWriter.PushSequence())
		{
			asnWriter.WriteObjectIdentifierForCrypto(text);
		}
		pssParamsAsn.MaskGenAlgorithm.Parameters = asnWriter.Encode();
		asnWriter.Reset();
		pssParamsAsn.Encode(asnWriter);
		AlgorithmIdentifierAsn algorithmIdentifierAsn = new AlgorithmIdentifierAsn
		{
			Algorithm = "1.2.840.113549.1.1.10",
			Parameters = asnWriter.Encode()
		};
		asnWriter.Reset();
		algorithmIdentifierAsn.Encode(asnWriter);
		return asnWriter.Encode();
	}

	public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
	{
		return _key.SignData(data, hashAlgorithm, _padding);
	}

	protected override PublicKey BuildPublicKey()
	{
		return RSAPkcs1X509SignatureGenerator.BuildPublicKey(_key);
	}
}
