using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct IssuerAndSerialNumberAsn
{
	internal ReadOnlyMemory<byte> Issuer;

	internal ReadOnlyMemory<byte> SerialNumber;

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out IssuerAndSerialNumberAsn decoded)
	{
		Decode(ref reader, Asn1Tag.Sequence, rebind, out decoded);
	}

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out IssuerAndSerialNumberAsn decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out IssuerAndSerialNumberAsn decoded)
	{
		decoded = default(IssuerAndSerialNumberAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		if (!asnValueReader.PeekTag().HasSameClassAndValue(new Asn1Tag(UniversalTagNumber.Sequence)))
		{
			throw new CryptographicException();
		}
		ReadOnlySpan<byte> other = asnValueReader.ReadEncodedValue();
		decoded.Issuer = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		other = asnValueReader.ReadIntegerBytes();
		decoded.SerialNumber = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader.ThrowIfNotEmpty();
	}
}
