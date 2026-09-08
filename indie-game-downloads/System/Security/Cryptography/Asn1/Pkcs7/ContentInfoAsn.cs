using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct ContentInfoAsn
{
	internal string ContentType;

	internal ReadOnlyMemory<byte> Content;

	internal readonly void Encode(AsnWriter writer)
	{
		Encode(writer, Asn1Tag.Sequence);
	}

	internal readonly void Encode(AsnWriter writer, Asn1Tag tag)
	{
		writer.PushSequence(tag);
		try
		{
			writer.WriteObjectIdentifier(ContentType);
		}
		catch (ArgumentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
		writer.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0));
		try
		{
			writer.WriteEncodedValue(Content.Span);
		}
		catch (ArgumentException inner2)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner2);
		}
		writer.PopSequence(new Asn1Tag(TagClass.ContextSpecific, 0));
		writer.PopSequence(tag);
	}

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out ContentInfoAsn decoded)
	{
		Decode(ref reader, Asn1Tag.Sequence, rebind, out decoded);
	}

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out ContentInfoAsn decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out ContentInfoAsn decoded)
	{
		decoded = default(ContentInfoAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		decoded.ContentType = asnValueReader.ReadObjectIdentifier();
		AsnValueReader asnValueReader2 = asnValueReader.ReadSequence(new Asn1Tag(TagClass.ContextSpecific, 0));
		ReadOnlySpan<byte> other = asnValueReader2.ReadEncodedValue();
		decoded.Content = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader2.ThrowIfNotEmpty();
		asnValueReader.ThrowIfNotEmpty();
	}
}
