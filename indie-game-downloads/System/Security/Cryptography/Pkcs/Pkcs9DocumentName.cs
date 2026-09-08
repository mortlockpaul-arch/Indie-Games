using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9DocumentName : Pkcs9AttributeObject
{
	private volatile string _lazyDocumentName;

	internal Pkcs9DocumentName(ReadOnlySpan<byte> encodedDocumentName)
		: base(Oids.DocumentNameOid.CopyOid(), encodedDocumentName)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyDocumentName = null;
	}
}
