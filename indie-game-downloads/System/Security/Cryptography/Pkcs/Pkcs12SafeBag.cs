using System.Collections.Generic;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal abstract class Pkcs12SafeBag
{
	internal sealed class UnknownBag : Pkcs12SafeBag
	{
		internal UnknownBag(string oidValue, ReadOnlyMemory<byte> bagValue)
			: base(oidValue, bagValue)
		{
		}
	}

	private readonly string _bagIdValue;

	private CryptographicAttributeObjectCollection _attributes;

	public ReadOnlyMemory<byte> EncodedBagValue { get; }

	public CryptographicAttributeObjectCollection Attributes
	{
		get
		{
			if (_attributes == null)
			{
				_attributes = new CryptographicAttributeObjectCollection();
			}
			return _attributes;
		}
		internal set
		{
			_attributes = value;
		}
	}

	protected Pkcs12SafeBag(string bagIdValue, ReadOnlyMemory<byte> encodedBagValue, bool skipCopy = false)
	{
		if (string.IsNullOrEmpty(bagIdValue))
		{
			throw new ArgumentNullException("bagIdValue");
		}
		PkcsHelpers.EnsureSingleBerValue(encodedBagValue.Span);
		_bagIdValue = bagIdValue;
		EncodedBagValue = (skipCopy ? encodedBagValue : ((ReadOnlyMemory<byte>)encodedBagValue.ToArray()));
	}

	internal void EncodeTo(AsnWriter writer)
	{
		writer.PushSequence();
		writer.WriteObjectIdentifierForCrypto(_bagIdValue);
		Asn1Tag value = new Asn1Tag(TagClass.ContextSpecific, 0);
		writer.PushSequence(value);
		writer.WriteEncodedValueForCrypto(EncodedBagValue.Span);
		writer.PopSequence(value);
		CryptographicAttributeObjectCollection attributes = _attributes;
		if (attributes != null && attributes.Count > 0)
		{
			List<AttributeAsn> list = PkcsHelpers.BuildAttributes(_attributes);
			writer.PushSetOf();
			foreach (AttributeAsn item in list)
			{
				item.Encode(writer);
			}
			writer.PopSetOf();
		}
		writer.PopSequence();
	}
}
