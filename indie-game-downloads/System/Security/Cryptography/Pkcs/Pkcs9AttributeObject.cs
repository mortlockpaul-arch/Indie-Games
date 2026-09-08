namespace System.Security.Cryptography.Pkcs;

internal class Pkcs9AttributeObject : AsnEncodedData
{
	public Pkcs9AttributeObject(AsnEncodedData asnEncodedData)
		: base(asnEncodedData)
	{
		if (asnEncodedData.Oid == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Arg_EmptyOrNullString_Named, "asnEncodedData.Oid"), "asnEncodedData");
		}
		if ((base.Oid.Value ?? throw new ArgumentException(System.SR.Format(System.SR.Arg_EmptyOrNullString_Named, "oid.Value"), "asnEncodedData")).Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Arg_EmptyOrNullString_Named, "oid.Value"), "asnEncodedData");
		}
	}

	internal Pkcs9AttributeObject(Oid oid, ReadOnlySpan<byte> encodedData)
		: this(new AsnEncodedData(oid, encodedData))
	{
	}

	internal Pkcs9AttributeObject(Oid oid)
	{
		base.Oid = oid;
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		ArgumentNullException.ThrowIfNull(asnEncodedData, "asnEncodedData");
		if (!(asnEncodedData is Pkcs9AttributeObject))
		{
			throw new ArgumentException(System.SR.Cryptography_Pkcs9_AttributeMismatch);
		}
		base.CopyFrom(asnEncodedData);
	}
}
