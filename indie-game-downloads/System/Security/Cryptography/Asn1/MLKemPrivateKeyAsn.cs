using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1;

internal struct MLKemPrivateKeyAsn
{
	internal ReadOnlyMemory<byte>? Seed;

	internal ReadOnlyMemory<byte>? ExpandedKey;

	internal MLKemPrivateKeyBothAsn? Both;

	internal readonly void Encode(AsnWriter writer)
	{
		bool flag = false;
		ReadOnlyMemory<byte> value;
		if (Seed.HasValue)
		{
			if (flag)
			{
				throw new CryptographicException();
			}
			value = Seed.Value;
			writer.WriteOctetString(value.Span, new Asn1Tag(TagClass.ContextSpecific, 0));
			flag = true;
		}
		if (ExpandedKey.HasValue)
		{
			if (flag)
			{
				throw new CryptographicException();
			}
			value = ExpandedKey.Value;
			writer.WriteOctetString(value.Span);
			flag = true;
		}
		if (Both.HasValue)
		{
			if (flag)
			{
				throw new CryptographicException();
			}
			Both.Value.Encode(writer);
			flag = true;
		}
		if (!flag)
		{
			throw new CryptographicException();
		}
	}

	internal static MLKemPrivateKeyAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		try
		{
			AsnValueReader reader = new AsnValueReader(encoded.Span, ruleSet);
			DecodeCore(ref reader, encoded, out var decoded);
			reader.ThrowIfNotEmpty();
			return decoded;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out MLKemPrivateKeyAsn decoded)
	{
		decoded = default(MLKemPrivateKeyAsn);
		Asn1Tag asn1Tag = reader.PeekTag();
		ReadOnlySpan<byte> span = rebind.Span;
		ReadOnlySpan<byte> value;
		int elementOffset;
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			if (reader.TryReadPrimitiveOctetString(out value, new Asn1Tag(TagClass.ContextSpecific, 0)))
			{
				decoded.Seed = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
			}
			else
			{
				decoded.Seed = reader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 0));
			}
			return;
		}
		if (asn1Tag.HasSameClassAndValue(Asn1Tag.PrimitiveOctetString))
		{
			if (reader.TryReadPrimitiveOctetString(out value))
			{
				decoded.ExpandedKey = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
			}
			else
			{
				decoded.ExpandedKey = reader.ReadOctetString();
			}
			return;
		}
		if (asn1Tag.HasSameClassAndValue(Asn1Tag.Sequence))
		{
			MLKemPrivateKeyBothAsn.Decode(ref reader, rebind, out var decoded2);
			decoded.Both = decoded2;
			return;
		}
		throw new CryptographicException();
	}
}
