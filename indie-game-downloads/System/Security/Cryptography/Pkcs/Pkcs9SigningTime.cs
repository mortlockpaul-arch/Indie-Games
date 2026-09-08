using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs9SigningTime : Pkcs9AttributeObject
{
	private DateTime? _lazySigningTime;

	internal Pkcs9SigningTime(ReadOnlySpan<byte> encodedSigningTime)
		: base(Oids.SigningTimeOid.CopyOid(), encodedSigningTime)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazySigningTime = null;
	}
}
