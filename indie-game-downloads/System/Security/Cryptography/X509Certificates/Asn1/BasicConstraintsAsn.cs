using System.Formats.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1;

internal struct BasicConstraintsAsn
{
	internal bool CA;

	internal int? PathLengthConstraint;

	private static ReadOnlySpan<byte> DefaultCA => new byte[3] { 1, 1, 0 };

	internal readonly void Encode(AsnWriter writer)
	{
		Encode(writer, Asn1Tag.Sequence);
	}

	internal readonly void Encode(AsnWriter writer, Asn1Tag tag)
	{
		writer.PushSequence(tag);
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, 3);
		asnWriter.WriteBoolean(CA);
		if (!asnWriter.EncodedValueEquals(DefaultCA))
		{
			asnWriter.CopyTo(writer);
		}
		if (PathLengthConstraint.HasValue)
		{
			writer.WriteInteger(PathLengthConstraint.Value);
		}
		writer.PopSequence(tag);
	}

	internal static BasicConstraintsAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static BasicConstraintsAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		try
		{
			AsnValueReader reader = new AsnValueReader(encoded.Span, ruleSet);
			DecodeCore(ref reader, expectedTag, out var decoded);
			reader.ThrowIfNotEmpty();
			return decoded;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, out BasicConstraintsAsn decoded)
	{
		decoded = default(BasicConstraintsAsn);
		AsnValueReader asnValueReader = reader.ReadSequence(expectedTag);
		if (asnValueReader.HasData && asnValueReader.PeekTag().HasSameClassAndValue(Asn1Tag.Boolean))
		{
			decoded.CA = asnValueReader.ReadBoolean();
		}
		else
		{
			decoded.CA = new AsnValueReader(DefaultCA, AsnEncodingRules.DER).ReadBoolean();
		}
		if (asnValueReader.HasData && asnValueReader.PeekTag().HasSameClassAndValue(Asn1Tag.Integer))
		{
			if (asnValueReader.TryReadInt32(out var value))
			{
				decoded.PathLengthConstraint = value;
			}
			else
			{
				asnValueReader.ThrowIfNotEmpty();
			}
		}
		asnValueReader.ThrowIfNotEmpty();
	}
}
