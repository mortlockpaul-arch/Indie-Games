using System.Formats.Asn1;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class MLDsaX509SignatureGenerator : X509SignatureGenerator
{
	private readonly MLDsa _key;

	internal MLDsaX509SignatureGenerator(MLDsa key)
	{
		_key = key;
	}

	public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, 16);
		asnWriter.PushSequence();
		asnWriter.WriteObjectIdentifier(_key.Algorithm.Oid);
		asnWriter.PopSequence();
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
		byte[] rawData = _key.ExportMLDsaPublicKey();
		return new PublicKey(oid, null, new AsnEncodedData(oid, rawData, skipCopy: true));
	}
}
