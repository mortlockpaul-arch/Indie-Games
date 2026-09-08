using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1;

internal struct RSAPrivateKeyAsn
{
	internal int Version;

	internal ReadOnlyMemory<byte> Modulus;

	internal ReadOnlyMemory<byte> PublicExponent;

	internal ReadOnlyMemory<byte> PrivateExponent;

	internal ReadOnlyMemory<byte> Prime1;

	internal ReadOnlyMemory<byte> Prime2;

	internal ReadOnlyMemory<byte> Exponent1;

	internal ReadOnlyMemory<byte> Exponent2;

	internal ReadOnlyMemory<byte> Coefficient;

	internal static RSAPrivateKeyAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static RSAPrivateKeyAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out RSAPrivateKeyAsn decoded)
	{
		decoded = default(RSAPrivateKeyAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		if (!asnValueReader.TryReadInt32(out decoded.Version))
		{
			asnValueReader.ThrowIfNotEmpty();
		}
		ReadOnlySpan<byte> other = asnValueReader.ReadIntegerBytes();
		decoded.Modulus = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.PublicExponent = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.PrivateExponent = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.Prime1 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.Prime2 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.Exponent1 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.Exponent2 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.Coefficient = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader.ThrowIfNotEmpty();
	}
}
