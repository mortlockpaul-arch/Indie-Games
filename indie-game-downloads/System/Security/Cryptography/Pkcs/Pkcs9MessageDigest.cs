using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9MessageDigest : Pkcs9AttributeObject
{
	private volatile byte[] _lazyMessageDigest;

	internal Pkcs9MessageDigest(ReadOnlySpan<byte> rawData)
		: base(Oids.MessageDigestOid.CopyOid(), rawData)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyMessageDigest = null;
	}
}
