using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct CertificateChoiceAsn
{
	internal ReadOnlyMemory<byte>? Certificate;

	internal ReadOnlyMemory<byte>? ExtendedCertificate;

	internal ReadOnlyMemory<byte>? AttributeCertificateV1;

	internal ReadOnlyMemory<byte>? AttributeCertificateV2;

	internal OtherCertificateFormat? OtherCertificateFormat;

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out CertificateChoiceAsn decoded)
	{
		try
		{
			DecodeCore(ref reader, rebind, out decoded);
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out CertificateChoiceAsn decoded)
	{
		decoded = default(CertificateChoiceAsn);
		Asn1Tag asn1Tag = reader.PeekTag();
		ReadOnlySpan<byte> span = rebind.Span;
		int elementOffset;
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(UniversalTagNumber.Sequence)))
		{
			ReadOnlySpan<byte> other = reader.ReadEncodedValue();
			decoded.Certificate = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
			return;
		}
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			ReadOnlySpan<byte> other = reader.ReadEncodedValue();
			decoded.ExtendedCertificate = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
			return;
		}
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 1)))
		{
			ReadOnlySpan<byte> other = reader.ReadEncodedValue();
			decoded.AttributeCertificateV1 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
			return;
		}
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 2)))
		{
			ReadOnlySpan<byte> other = reader.ReadEncodedValue();
			decoded.AttributeCertificateV2 = (span.Overlaps(other, out elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
			return;
		}
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 3)))
		{
			System.Security.Cryptography.Asn1.Pkcs7.OtherCertificateFormat.Decode(ref reader, new Asn1Tag(TagClass.ContextSpecific, 3), rebind, out var decoded2);
			decoded.OtherCertificateFormat = decoded2;
			return;
		}
		throw new CryptographicException();
	}
}
