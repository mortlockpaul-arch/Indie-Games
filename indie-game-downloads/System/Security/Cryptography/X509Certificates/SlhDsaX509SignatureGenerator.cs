using System.Formats.Asn1;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class SlhDsaX509SignatureGenerator : X509SignatureGenerator
{
	private readonly SlhDsa _key;

	internal SlhDsaX509SignatureGenerator(SlhDsa key)
	{
		_key = key;
	}

	public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, 16);
		using (asnWriter.PushSequence())
		{
			asnWriter.WriteObjectIdentifier(_key.Algorithm.Oid);
		}
		return asnWriter.Encode();
	}

	public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		return _key.SignData(data);
	}

	protected override PublicKey BuildPublicKey()
	{
		Oid oid = new Oid(_key.Algorithm.Oid, null);
		byte[] rawData = _key.ExportSlhDsaPublicKey();
		return new PublicKey(oid, null, new AsnEncodedData(oid, rawData, skipCopy: true));
	}
}
