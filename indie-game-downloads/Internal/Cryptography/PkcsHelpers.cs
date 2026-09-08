using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;

namespace Internal.Cryptography;

internal static class PkcsHelpers
{
	internal static List<AttributeAsn> BuildAttributes(CryptographicAttributeObjectCollection attributes)
	{
		List<AttributeAsn> list = new List<AttributeAsn>();
		if (attributes == null || attributes.Count == 0)
		{
			return list;
		}
		foreach (CryptographicAttributeObject attribute in attributes)
		{
			AttributeAsn item = new AttributeAsn
			{
				AttrType = attribute.Oid.Value,
				AttrValues = new ReadOnlyMemory<byte>[attribute.Values.Count]
			};
			for (int i = 0; i < attribute.Values.Count; i++)
			{
				item.AttrValues[i] = attribute.Values[i].RawData;
			}
			list.Add(item);
		}
		return list;
	}

	public static void EnsureSingleBerValue(ReadOnlySpan<byte> source)
	{
		if (!AsnDecoder.TryReadEncodedValue(source, AsnEncodingRules.BER, out var _, out var _, out var _, out var bytesConsumed) || bytesConsumed != source.Length)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
	}

	public static int FirstBerValueLength(ReadOnlySpan<byte> source)
	{
		if (!AsnDecoder.TryReadEncodedValue(source, AsnEncodingRules.BER, out var _, out var _, out var _, out var bytesConsumed))
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		return bytesConsumed;
	}

	public static ReadOnlyMemory<byte> DecodeOctetStringAsMemory(ReadOnlyMemory<byte> encodedOctetString)
	{
		return Helpers.DecodeOctetStringAsMemory(encodedOctetString);
	}

	internal static string GetOidFromHashAlgorithm(HashAlgorithmName algName)
	{
		if (algName == HashAlgorithmName.MD5)
		{
			return "1.2.840.113549.2.5";
		}
		if (algName == HashAlgorithmName.SHA1)
		{
			return "1.3.14.3.2.26";
		}
		if (algName == HashAlgorithmName.SHA256)
		{
			return "2.16.840.1.101.3.4.2.1";
		}
		if (algName == HashAlgorithmName.SHA384)
		{
			return "2.16.840.1.101.3.4.2.2";
		}
		if (algName == HashAlgorithmName.SHA512)
		{
			return "2.16.840.1.101.3.4.2.3";
		}
		if (algName == HashAlgorithmName.SHA3_256)
		{
			return "2.16.840.1.101.3.4.2.8";
		}
		if (algName == HashAlgorithmName.SHA3_384)
		{
			return "2.16.840.1.101.3.4.2.9";
		}
		if (algName == HashAlgorithmName.SHA3_512)
		{
			return "2.16.840.1.101.3.4.2.10";
		}
		throw new CryptographicException(System.SR.Cryptography_Cms_UnknownAlgorithm, algName.Name);
	}

	[return: NotNullIfNotNull("oid")]
	public static Oid CopyOid(this Oid oid)
	{
		return oid;
	}

	internal static CryptographicAttributeObjectCollection MakeAttributeCollection(AttributeAsn[] attributes)
	{
		CryptographicAttributeObjectCollection cryptographicAttributeObjectCollection = new CryptographicAttributeObjectCollection();
		if (attributes == null)
		{
			return cryptographicAttributeObjectCollection;
		}
		foreach (AttributeAsn attribute in attributes)
		{
			cryptographicAttributeObjectCollection.AddWithoutMerge(MakeAttribute(attribute));
		}
		return cryptographicAttributeObjectCollection;
	}

	internal static CryptographicAttributeObject MakeAttribute(AttributeAsn attribute)
	{
		Oid oid = new Oid(attribute.AttrType);
		AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection();
		ReadOnlyMemory<byte>[] attrValues = attribute.AttrValues;
		foreach (ReadOnlyMemory<byte> readOnlyMemory in attrValues)
		{
			asnEncodedDataCollection.Add(CreateBestPkcs9AttributeObjectAvailable(oid, readOnlyMemory.ToArray()));
		}
		return new CryptographicAttributeObject(oid, asnEncodedDataCollection);
	}

	public static Pkcs9AttributeObject CreateBestPkcs9AttributeObjectAvailable(Oid oid, ReadOnlySpan<byte> encodedAttribute)
	{
		return oid.Value switch
		{
			"1.3.6.1.4.1.311.88.2.1" => new Pkcs9DocumentName(encodedAttribute), 
			"1.3.6.1.4.1.311.88.2.2" => new Pkcs9DocumentDescription(encodedAttribute), 
			"1.2.840.113549.1.9.5" => new Pkcs9SigningTime(encodedAttribute), 
			"1.2.840.113549.1.9.3" => new Pkcs9ContentType(encodedAttribute), 
			"1.2.840.113549.1.9.4" => new Pkcs9MessageDigest(encodedAttribute), 
			"1.2.840.113549.1.9.21" => new Pkcs9LocalKeyId
			{
				RawData = encodedAttribute.ToArray()
			}, 
			_ => new Pkcs9AttributeObject(oid, encodedAttribute), 
		};
	}

	public static AttributeAsn[] NormalizeAttributeSet(AttributeAsn[] setItems)
	{
		byte[] encodedValue;
		return NormalizeAttributeSet(setItems, out encodedValue);
	}

	public static AttributeAsn[] NormalizeAttributeSet(AttributeAsn[] setItems, out byte[] encodedValue)
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.PushSetOf();
		foreach (AttributeAsn attributeAsn in setItems)
		{
			attributeAsn.Encode(asnWriter);
		}
		asnWriter.PopSetOf();
		byte[] array = (encodedValue = asnWriter.Encode());
		try
		{
			AsnValueReader reader = new AsnValueReader(array, AsnEncodingRules.DER).ReadSetOf();
			AttributeAsn[] array2 = new AttributeAsn[setItems.Length];
			int num = 0;
			while (reader.HasData)
			{
				AttributeAsn.Decode(ref reader, array, out var decoded);
				array2[num] = decoded;
				num++;
			}
			return array2;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}
}
