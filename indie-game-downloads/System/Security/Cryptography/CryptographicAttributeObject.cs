using Internal.Cryptography;

namespace System.Security.Cryptography;

internal sealed class CryptographicAttributeObject
{
	private readonly Oid _oid;

	public Oid Oid => _oid.CopyOid();

	public AsnEncodedDataCollection Values { get; }

	public CryptographicAttributeObject(Oid oid, AsnEncodedDataCollection values)
	{
		_oid = oid.CopyOid();
		if (values == null)
		{
			Values = new AsnEncodedDataCollection();
			return;
		}
		foreach (AsnEncodedData value in values)
		{
			if (value.Oid == null)
			{
				throw new ArgumentException(System.SR.Argument_InvalidOidValue, "values");
			}
			if (!string.Equals(value.Oid.Value, oid.Value, StringComparison.Ordinal))
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.InvalidOperation_WrongOidInAsnCollection, oid.Value, value.Oid.Value));
			}
		}
		Values = values;
	}
}
