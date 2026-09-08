using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct OtherCertificateFormat
{
	internal string OtherCertFormat;

	internal ReadOnlyMemory<byte> OtherCert;

	internal static void Decode(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out OtherCertificateFormat decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out OtherCertificateFormat decoded)
	{
		decoded = default(OtherCertificateFormat);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		decoded.OtherCertFormat = asnValueReader.ReadObjectIdentifier();
		ReadOnlySpan<byte> other = asnValueReader.ReadEncodedValue();
		decoded.OtherCert = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
		asnValueReader.ThrowIfNotEmpty();
	}
}
