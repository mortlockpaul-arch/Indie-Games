using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9LocalKeyId : Pkcs9AttributeObject
{
	private byte[] _lazyKeyId;

	public Pkcs9LocalKeyId()
		: base(Oids.LocalKeyIdOid.CopyOid())
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyKeyId = null;
	}
}
