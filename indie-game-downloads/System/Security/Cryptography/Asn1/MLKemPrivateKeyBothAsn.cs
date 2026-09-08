using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1;

internal struct MLKemPrivateKeyBothAsn
{
	internal ReadOnlyMemory<byte> Seed;

	internal ReadOnlyMemory<byte> ExpandedKey;

	internal readonly void Encode(AsnWriter writer)
	{
		Encode(writer, Asn1Tag.Sequence);
	}

	internal readonly void Encode(AsnWriter writer, Asn1Tag tag)
	{
		writer.PushSequence(tag);
		writer.WriteOctetString(Seed.Span);
		writer.WriteOctetString(ExpandedKey.Span);
		writer.PopSequence(tag);
	}

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out MLKemPrivateKeyBothAsn decoded)
	{
		Decode(ref reader, Asn1Tag.Sequence, rebind, out decoded);
	}

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out MLKemPrivateKeyBothAsn decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out MLKemPrivateKeyBothAsn decoded)
	{
		decoded = default(MLKemPrivateKeyBothAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		int elementOffset;
		if (asnValueReader.TryReadPrimitiveOctetString(out var value))
		{
			decoded.Seed = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
		}
		else
		{
			decoded.Seed = asnValueReader.ReadOctetString();
		}
		if (asnValueReader.TryReadPrimitiveOctetString(out value))
		{
			decoded.ExpandedKey = (span.Overlaps(value, out elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
		}
		else
		{
			decoded.ExpandedKey = asnValueReader.ReadOctetString();
		}
		asnValueReader.ThrowIfNotEmpty();
	}
}
