using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9ContentType : Pkcs9AttributeObject
{
	private volatile Oid _lazyContentType;

	internal Pkcs9ContentType(ReadOnlySpan<byte> rawData)
		: base(Oids.ContentTypeOid.CopyOid(), rawData)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyContentType = null;
	}
}
