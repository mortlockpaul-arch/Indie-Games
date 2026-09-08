using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1;

internal struct RSAPublicKeyAsn
{
	internal ReadOnlyMemory<byte> Modulus;

	internal ReadOnlyMemory<byte> PublicExponent;

	internal static RSAPublicKeyAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static RSAPublicKeyAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out RSAPublicKeyAsn decoded)
	{
		decoded = default(RSAPublicKeyAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		ReadOnlySpan<byte> other = asnValueReader.ReadIntegerBytes();
		decoded.Modulus = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.PublicExponent = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader.ThrowIfNotEmpty();
	}
}
