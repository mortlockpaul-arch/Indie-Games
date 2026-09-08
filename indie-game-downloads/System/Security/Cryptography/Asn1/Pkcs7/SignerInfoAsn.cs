using System.Collections.Generic;
using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct SignerInfoAsn
{
	internal int Version;

	internal SignerIdentifierAsn Sid;

	internal AlgorithmIdentifierAsn DigestAlgorithm;

	internal ReadOnlyMemory<byte>? SignedAttributes;

	internal AlgorithmIdentifierAsn SignatureAlgorithm;

	internal ReadOnlyMemory<byte> SignatureValue;

	internal AttributeAsn[] UnsignedAttributes;

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out SignerInfoAsn decoded)
	{
		Decode(ref reader, Asn1Tag.Sequence, rebind, out decoded);
	}

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out SignerInfoAsn decoded)
	{
		try
		{
			DecodeCore(ref reader, expectedTag, rebind, out decoded);
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out SignerInfoAsn decoded)
	{
		decoded = default(SignerInfoAsn);
		AsnValueReader reader2 = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		if (!reader2.TryReadInt32(out decoded.Version))
		{
			reader2.ThrowIfNotEmpty();
		}
		SignerIdentifierAsn.Decode(ref reader2, rebind, out decoded.Sid);
		AlgorithmIdentifierAsn.Decode(ref reader2, rebind, out decoded.DigestAlgorithm);
		ReadOnlySpan<byte> value;
		int elementOffset;
		if (reader2.HasData && reader2.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			value = reader2.ReadEncodedValue();
			decoded.SignedAttributes = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
		}
		AlgorithmIdentifierAsn.Decode(ref reader2, rebind, out decoded.SignatureAlgorithm);
		if (reader2.TryReadPrimitiveOctetString(out value))
		{
			decoded.SignatureValue = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
		}
		else
		{
			decoded.SignatureValue = reader2.ReadOctetString();
		}
		if (reader2.HasData && reader2.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 1)))
		{
			AsnValueReader reader3 = reader2.ReadSetOf(new Asn1Tag(TagClass.ContextSpecific, 1));
			List<AttributeAsn> list = new List<AttributeAsn>();
			while (reader3.HasData)
			{
				AttributeAsn.Decode(ref reader3, rebind, out var decoded2);
				list.Add(decoded2);
			}
			decoded.UnsignedAttributes = list.ToArray();
		}
		reader2.ThrowIfNotEmpty();
	}
}
