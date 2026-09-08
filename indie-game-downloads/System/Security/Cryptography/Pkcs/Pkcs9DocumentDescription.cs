using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9DocumentDescription : Pkcs9AttributeObject
{
	private volatile string _lazyDocumentDescription;

	internal Pkcs9DocumentDescription(ReadOnlySpan<byte> encodedDocumentDescription)
		: base(Oids.DocumentDescriptionOid.CopyOid(), encodedDocumentDescription)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyDocumentDescription = null;
	}
}
