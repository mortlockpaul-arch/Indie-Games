using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct EncapsulatedContentInfoAsn
{
	internal string ContentType;

	internal ReadOnlyMemory<byte>? Content;

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out EncapsulatedContentInfoAsn decoded)
	{
		Decode(ref reader, Asn1Tag.Sequence, rebind, out decoded);
	}

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out EncapsulatedContentInfoAsn decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out EncapsulatedContentInfoAsn decoded)
	{
		decoded = default(EncapsulatedContentInfoAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		decoded.ContentType = asnValueReader.ReadObjectIdentifier();
		if (asnValueReader.HasData && asnValueReader.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			AsnValueReader asnValueReader2 = asnValueReader.ReadSequence(new Asn1Tag(TagClass.ContextSpecific, 0));
			ReadOnlySpan<byte> other = asnValueReader2.ReadEncodedValue();
			decoded.Content = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
			asnValueReader2.ThrowIfNotEmpty();
		}
		asnValueReader.ThrowIfNotEmpty();
	}
}
