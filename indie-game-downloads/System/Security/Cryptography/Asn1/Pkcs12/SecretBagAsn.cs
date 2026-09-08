using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs12;

internal struct SecretBagAsn
{
	internal string SecretTypeId;

	internal ReadOnlyMemory<byte> SecretValue;

	internal static SecretBagAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static SecretBagAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		try
		{
			AsnValueReader reader = new AsnValueReader(encoded.Span, ruleSet);
			DecodeCore(ref reader, expectedTag, encoded, out var decoded);
			reader.ThrowIfNotEmpty();
			return decoded;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out SecretBagAsn decoded)
	{
		decoded = default(SecretBagAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		decoded.SecretTypeId = asnValueReader.ReadObjectIdentifier();
		AsnValueReader asnValueReader2 = asnValueReader.ReadSequence(new Asn1Tag(TagClass.ContextSpecific, 0));
		ReadOnlySpan<byte> other = asnValueReader2.ReadEncodedValue();
		decoded.SecretValue = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader2.ThrowIfNotEmpty();
		asnValueReader.ThrowIfNotEmpty();
	}
}
