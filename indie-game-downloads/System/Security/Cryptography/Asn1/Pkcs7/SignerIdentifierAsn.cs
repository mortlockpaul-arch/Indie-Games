using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct SignerIdentifierAsn
{
	internal IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

	internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;

	internal static void Decode(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out SignerIdentifierAsn decoded)
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

	private static void DecodeCore(ref AsnValueReader reader, ReadOnlyMemory<byte> rebind, out SignerIdentifierAsn decoded)
	{
		decoded = default(SignerIdentifierAsn);
		Asn1Tag asn1Tag = reader.PeekTag();
		ReadOnlySpan<byte> span = rebind.Span;
		if (asn1Tag.HasSameClassAndValue(Asn1Tag.Sequence))
		{
			IssuerAndSerialNumberAsn.Decode(ref reader, rebind, out var decoded2);
			decoded.IssuerAndSerialNumber = decoded2;
			return;
		}
		if (asn1Tag.HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			if (reader.TryReadPrimitiveOctetString(out var value, new Asn1Tag(TagClass.ContextSpecific, 0)))
			{
				decoded.SubjectKeyIdentifier = (span.Overlaps(value, out var elementOffset) ? rebind.Slice(elementOffset, value.Length) : ((ReadOnlyMemory<byte>)value.ToArray()));
			}
			else
			{
				decoded.SubjectKeyIdentifier = reader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 0));
			}
			return;
		}
		throw new CryptographicException();
	}
}
